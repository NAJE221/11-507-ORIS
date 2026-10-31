using System.Net;
using http_server.Data;
using http_server.Domain;
using http_server.Framework.Abstractions.Interfaces;
using http_server.Framework.Attributes;

namespace http_server.Framework.HttpControllers;

[HttpController("trips")]
public class TripsController(HttpListenerContext context, ITripService service)
{
    [Post("create")]
    public void PrintTrips()
    {
        TripsData.TripsList.Add(new Trip("Новое место прибытия", new Bus("Новый автобус")));
        Console.WriteLine(TripsData.TripsList[^1]);
    }   

    [Get("details")]
    public async Task GetDetailsTrips()
    {
        await service.GetTripById(context);
    }
}