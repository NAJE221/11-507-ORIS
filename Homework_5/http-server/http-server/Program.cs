using System.Net;
using http_server.Framework.Abstractions.Base;
using http_server.Framework.Abstractions.Interfaces;
using http_server.Framework.Handlers;
using http_server.Framework.Http;
using http_server.Framework.Routing;
using http_server.Framework.Services;

var listener = new HttpListener();

Handler fileHandler = new FileHandler(new ResourceService());

IResourceService resourceService = new ResourceService();
Handler controllerHandler = new ControllerHandler(resourceService);

RouteRegistry.Register();
var myServer = new HttpServer(listener,fileHandler,controllerHandler);

myServer.Run();
