using http_server.Domain;

namespace http_server.Data;

public class TripsData
{
    public static List<Trip> TripsList = new()
    {
        new Trip("Казань", new Bus("Автобус")),
        new Trip("Москва", new Plane("Автобус")),
        new Trip("Санкт-Петербург", new Train("Поезд"))
    };
}