namespace http_server.Framework.Abstractions;
//Тут была s, законментил , иначе не скомпилируется
public abstract class Transport(string name)
{
    public string Name = name;
}
