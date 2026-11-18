using MiniHttpServer.ORM.Abstractions;

namespace MiniHttpServer.Data;

public class OrmContext(IDbDriver dbDriver)
{
    IDbDriver _dbDriver = dbDriver;
}