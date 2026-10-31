using System.Net;
using http_server.Data;
using http_server.Framework.Abstractions.Interfaces;
using http_server.Framework.Factories;

namespace http_server.Framework.Services;

public class TripService(IResourceService resourceService) : ITripService
{
    private static void PrintTrip(HttpListenerContext context)
    {
        var tripRequest = TripsRequestMapper.Map(context);
        Console.WriteLine(TripsData.TripsList[tripRequest.id]);
    }

    
    public async Task GetTripById(HttpListenerContext context)
    {
        try
        {
            PrintTrip(context);
            await resourceService.SendErrorPage("404", context);
        }
        catch (Exception)
        {
            Console.WriteLine("Неправильно введен ID");
            await resourceService.SendErrorPage("404", context);
        }
    }
}