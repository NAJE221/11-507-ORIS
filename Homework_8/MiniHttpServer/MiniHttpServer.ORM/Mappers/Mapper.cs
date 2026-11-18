using System.Data.Common;
using MiniHttpServer.ORM.Abstractions;

namespace MiniHttpServer.ORM.Mappers;

public static class Mapper
{
    public static DbParameter[] MapToParameters<T>(
        T entity, 
        IDbDriver driver,
        bool primaryKeyOnly = false)  
        where T : class
    {
        var parameters = primaryKeyOnly switch
        {
            false => ReflectionHelper
                .GetWritablePropertiesWithoutId(typeof(T)),
            true => ReflectionHelper
                .GetPropertyPrimaryKey(typeof(T))
        };
        
        return parameters.Select((p , i) 
            => driver.CreateParameter(
                $"@p{i}", 
                p.GetValue(entity) ?? DBNull.Value))
                .ToArray();
    }
    
    public static T MapToEntity<T>(DbDataReader reader)  
        where T : class , new()
    {
        var entity = new T();
        
        var writableProperties = 
            ReflectionHelper.GetWritableProperties(typeof(T))
                .ToArray();

        foreach (var property in writableProperties)
        {
            var columnName = ReflectionHelper.GetColumnName(property);
            
            var ordinal = reader.GetOrdinal(columnName);
            var value =  reader.GetValue(ordinal);

            if (value == DBNull.Value)
                continue;
            
            property.SetValue(entity, value);
        }
        return entity;
    }
}