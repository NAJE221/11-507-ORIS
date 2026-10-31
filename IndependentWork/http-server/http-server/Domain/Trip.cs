using http_server.Framework.Abstractions;

namespace http_server.Domain;

public class Trip(string placeArrival, Transport transport)
{
    public string PlaceArrival = placeArrival;
    public string Status = "planed";
    public Transport Transport = transport;
}