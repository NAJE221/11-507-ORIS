using System.Reflection;
using MiniHttpServer.ORM.Attributes;

namespace MiniHttpServer.ORM;

public static class ReflectionHelper
{
    public static PropertyInfo[] GetPropertyPrimaryKey(Type type) =>
        type.GetProperties()
            .Where(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null)
            .ToArray();

    public static PropertyInfo[] GetWritablePropertiesWithoutId(Type type) =>
        GetWritableProperties(type)
            .Where(p => p.GetCustomAttribute<PrimaryKeyAttribute>() == null)
            .ToArray();
    
    public static IEnumerable<PropertyInfo> GetWritableProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p is { CanWrite: true, SetMethod.IsPublic: true });
    }
    
    public static IEnumerable<string> GetColumnNames(PropertyInfo[] properties)
    {
        return properties.Select(p => 
            p.GetCustomAttribute<ColumnNameAttribute>()?.ColumnName 
            ?? p.Name.ToLowerInvariant());
    }

    public static string GetColumnName(PropertyInfo property)
    {
        return property.GetCustomAttribute<ColumnNameAttribute>()?.ColumnName
            ?? property.Name.ToLowerInvariant();
    }

    public static string GetTableName<T>()
    {
        var tableNameAttribute = typeof(T).GetCustomAttribute<TableNameAttribute>();
        
        return tableNameAttribute?.TableName ?? typeof(T).Name.ToLower();
    }
}