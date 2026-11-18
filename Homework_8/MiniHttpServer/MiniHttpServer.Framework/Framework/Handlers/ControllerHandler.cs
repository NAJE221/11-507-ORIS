using System.Net;
using System.Web;
using MiniHttpServer.Framework.Framework.Abstractions;
using MiniHttpServer.Framework.Framework.Factories;
using MiniHttpServer.Framework.Framework.Routing;
using MiniHttpServer.Framework.Framework.Storages;

namespace MiniHttpServer.Framework.Framework.Handlers;

internal class ControllerHandler(IResourceService resourceService , ServiceStorage storage) : Handler
{
    public override async Task HandlerRequest(HttpListenerContext context)
    {
        var segments = context.Request.Url?.AbsolutePath
            .Trim('/')
            .Split('/');
        
        if (segments is null)
        {
            await resourceService.SendErrorPage(404, context);
            return;
        }
     
        var request = context.Request;
        
        var requestMethod =  request.HttpMethod;
        var controllerRoute = segments.Length > 0 ? segments[0] : string.Empty;
        var methodRoute = segments.Length > 1 ? segments[1] : string.Empty;
        
        var routeKey = $"{requestMethod}|{controllerRoute}|{methodRoute}";
   
        if (!RouteRegistry.RouteDescriptors.TryGetValue(
                routeKey, out var routeDescriptor))
        {
            await resourceService.SendErrorPage(404, context);
            return;
        }
        
        var controller = ControllerFactory.Create(
            routeDescriptor.Controller, 
            context, 
            storage);
        
        var method = routeDescriptor.Method;
        
        var resultCheckNull = await resourceService.CheckNull(context, 404, controller, method);
        if (!resultCheckNull) return;
        
        
        var methodParams = routeDescriptor.MethodsParams;
        var requestParameters = new object[methodParams.Length];

        switch (requestMethod.ToLower())
        {
            case "post" or "put" or "path":
            {
                using var stream = new StreamReader(request.InputStream, request.ContentEncoding);
                var body = await stream.ReadToEndAsync();
                var bodyParams = HttpUtility.ParseQueryString(body);

                for (var i = 0; i < requestParameters.Length; i++)
                {
                    requestParameters[i] = bodyParams[methodParams[i].Name] ?? string.Empty;
                }
            } break;
        }
        
        var result = method.Invoke(controller, requestParameters);
        if (result is Task task)
        {
            await task;
        }
        
    }
}