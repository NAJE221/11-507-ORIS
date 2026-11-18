
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using MiniHttpServer.ORM.Abstractions;
using MiniHttpServer.ORM.Attributes;
using MiniHttpServer.ORM.Mappers;

namespace MiniHttpServer.ORM;

public class OrmContext(IDbDriver dbDriver) : IOrmContext
{
    private readonly IDbDriver _dbDriver = dbDriver 
                        ?? throw new ArgumentNullException(nameof(dbDriver), "Не обнаружено драйвера");
    
    private readonly ISqlBuilder _sqlBuilder = dbDriver.CreateSqlBuilder()
                                               ?? throw new ArgumentNullException(nameof(_sqlBuilder), "Не обнаружено билдера");


    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> GetAllAsync<T>(
        CancellationToken cancellationToken = default) 
        where T : class, new()
    {
        List<T> entities = [];

        var tableName = ReflectionHelper.GetTableName<T>();
        
        await using var connection = _dbDriver.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var query = _sqlBuilder.BuildSelect(tableName);
        await using var command = _dbDriver.CreateCommand(query, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            entities.Add(Mapper.MapToEntity<T>(reader));
        }
        return entities;
    }

    /// <inheritdoc />
    public async Task<int> InsertAsync<T>(
        T entity, 
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);
        
        var tableName = ReflectionHelper.GetTableName<T>();
        
        await using var connection = _dbDriver.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        
        var query = _sqlBuilder.BuildInsert(tableName, entity);
        
        await using var command = _dbDriver.CreateCommand(query, connection);
        
        var parameters = Mapper.MapToParameters(entity, _dbDriver);

        foreach (var parameter in parameters)
        {
            command.Parameters.Add(parameter);
        }
        
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> UpdateAsync<T>(
        T entity,
        CancellationToken cancellationToken = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        var tableName = ReflectionHelper.GetTableName<T>();
        
        await using var connection = _dbDriver.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var query = _sqlBuilder.BuildUpdate(tableName, entity);
        await using var command = _dbDriver.CreateCommand(query, connection);

        var properties = ReflectionHelper.GetWritablePropertiesWithoutId(typeof(T));
        var pkProperties = ReflectionHelper.GetPropertyPrimaryKey(typeof(T));

        var i = 0;
        foreach (var p in properties.Concat(pkProperties))
        {
            command.Parameters.Add(
                _dbDriver.CreateParameter($"@p{i++}", p.GetValue(entity)));
        }

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> DeleteAsync<T>(
        object[] id,
        CancellationToken cancellationToken = default) where T : class
    {
        CheckKey<T>(id);   

        var tableName = ReflectionHelper.GetTableName<T>();
        
        await using var connection = _dbDriver.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var query = _sqlBuilder.BuildDelete<T>(tableName);
        await using var command = _dbDriver.CreateCommand(query, connection);

        for (var i = 0; i < id.Length; i++)
            command.Parameters.Add(_dbDriver.CreateParameter($"@p{i}", id[i]));

        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync<T>(
        object[] id, 
        CancellationToken cancellationToken = default) where T : class, new()
    {
        CheckKey<T>(id); 
        
        var tableName = ReflectionHelper.GetTableName<T>();
        
        await using var connection = _dbDriver.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        
        var query = _sqlBuilder.BuildSelectById<T>(tableName);
        
        await using var command = _dbDriver.CreateCommand(query, connection);
        
        for (var i = 0; i < id.Length; i++)
            command.Parameters.Add(_dbDriver.CreateParameter($"@p{i}", id[i]));
        
        await using var reader = await command
            .ExecuteReaderAsync(cancellationToken);
        
        if (await reader.ReadAsync(cancellationToken))
            return Mapper.MapToEntity<T>(reader);
        
        return null;
    }
    
    private static void CheckKey<T>(object[] id)
    {
        ArgumentNullException.ThrowIfNull(id);
        var count = ReflectionHelper.GetPropertyPrimaryKey(typeof(T)).Length;
        if (id.Length != count)
            throw new ArgumentException(
                $"Ожидалось значений ключа: {count}, получено: {id.Length}.");
    }
}
