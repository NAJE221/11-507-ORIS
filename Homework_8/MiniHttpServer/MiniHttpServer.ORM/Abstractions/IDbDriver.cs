using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace MiniHttpServer.ORM.Abstractions;

public interface IDbDriver
{
    public DbConnection CreateConnection();
    public DbCommand CreateCommand(string qwertyCommand, DbConnection connection);
    public DbParameter CreateParameter(string parameterName, object? parameterValue);
    public ISqlBuilder CreateSqlBuilder();
}