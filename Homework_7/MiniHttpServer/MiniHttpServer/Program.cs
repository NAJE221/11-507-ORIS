using MiniHttpServer.Framework.Framework.Builder;

var builder = await FrameworkBuilder.CreateBuilder("server.json" , "Server");

await builder.Server.RunAsync();


