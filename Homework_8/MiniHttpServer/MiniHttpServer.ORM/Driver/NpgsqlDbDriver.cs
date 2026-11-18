using System.Data.Common;
using MiniHttpServer.ORM.Abstractions;
using MiniHttpServer.ORM.Builder;
using Npgsql;

namespace MiniHttpServer.ORM.Driver;

public class NpgsqlDbDriver : IDbDriver
{
    private readonly string _connectionString;

    public NpgsqlDbDriver(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException($"Строка подключения: {connectionString} оказалась битой.");
        
        _connectionString = connectionString;
    }
    
    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    public DbCommand CreateCommand(string qwertyCommand, DbConnection connection)
    {
        if (connection is NpgsqlConnection npgsqlConnection)
            return new NpgsqlCommand(qwertyCommand, npgsqlConnection);

        throw new ArgumentException
            ($"Переданный объект {nameof(connection)} несовместим с драйвером NpgsqlDbDriver");
    }

    public DbParameter CreateParameter(string parameterName, object? parameterValue)
    {
        return new NpgsqlParameter(parameterName, parameterValue ?? DBNull.Value) ;
    }

    public ISqlBuilder CreateSqlBuilder()
    {
        return new NpgSqlBuilder();
    }
}

