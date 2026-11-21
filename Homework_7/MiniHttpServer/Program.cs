using MiniHttpServer.Framework.Framework.Builder;

var builder = await FrameworkBuilder.CreateBuilder("Secrets/server.json" , "Server");

await builder.Server.RunAsync();


