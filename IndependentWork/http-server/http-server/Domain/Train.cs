using http_server.Framework.Abstractions;
using MimeKit.Encodings;

namespace http_server.Domain;

public class Train(string name) : Transport(name)
{
}