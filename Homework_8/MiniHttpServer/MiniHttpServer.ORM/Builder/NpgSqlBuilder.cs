using System.Text;
using System.Text.RegularExpressions;
using MiniHttpServer.ORM.Abstractions;

namespace MiniHttpServer.ORM.Builder;

public class NpgSqlBuilder : ISqlBuilder
{
    
    /// <inheritdoc />
    public string BuildSelect(string tableName)
    {
        var verifiedTableName = EscapeIdentifier(tableName);
        return $"SELECT * FROM {verifiedTableName}";
    }
    
   
    public string BuildSelectById<T>(string tableName) where T : class
    {
        var pkProperties = ReflectionHelper.GetPropertyPrimaryKey(typeof(T));
        if (pkProperties.Length == 0)
            throw new ArgumentException("Не найден первичный ключ.");

        var where = string.Join(" AND ",
            ReflectionHelper.GetColumnNames(pkProperties)
                .Select((c, i) => $"{EscapeIdentifier(c)} = @p{i}"));

        return $"SELECT * FROM {EscapeIdentifier(tableName)} WHERE {where}";
    }

    public string BuildInsert<T>(string tableName, T entity) where T : class
    {
        CheckNull(entity);

        var properties = ReflectionHelper.GetWritablePropertiesWithoutId(typeof(T));
        CheckEmpty(properties);

        var columns = string.Join(", ",
            ReflectionHelper.GetColumnNames(properties).Select(EscapeIdentifier));
        var parameters = string.Join(", ", properties.Select((_, i) => $"@p{i}"));

        return $"INSERT INTO {EscapeIdentifier(tableName)} ({columns}) VALUES ({parameters})";
    }

    public string BuildUpdate<T>(string tableName, T entity) where T : class
    {
        CheckNull(entity);

        var properties = ReflectionHelper.GetWritablePropertiesWithoutId(typeof(T));
        CheckEmpty(properties);

        var pkProperties = ReflectionHelper.GetPropertyPrimaryKey(typeof(T));
        if (pkProperties.Length == 0)
            throw new ArgumentException("Не найден первичный ключ.");

        var set = string.Join(", ",
            ReflectionHelper.GetColumnNames(properties)
                .Select((c, i) => $"{EscapeIdentifier(c)} = @p{i}"));

        var offset = properties.Length;
        var where = string.Join(" AND ",
            ReflectionHelper.GetColumnNames(pkProperties)
                .Select((c, i) => $"{EscapeIdentifier(c)} = @p{offset + i}"));

        return $"UPDATE {EscapeIdentifier(tableName)} SET {set} WHERE {where}";
    }

    public string BuildDelete<T>(string tableName) where T : class
    {
        var pkProperties = ReflectionHelper.GetPropertyPrimaryKey(typeof(T));
        if (pkProperties.Length == 0)
            throw new ArgumentException("Не найден первичный ключ.");

        var where = string.Join(" AND ",
            ReflectionHelper.GetColumnNames(pkProperties)
                .Select((c, i) => $"{EscapeIdentifier(c)} = @p{i}"));

        return $"DELETE FROM {EscapeIdentifier(tableName)} WHERE {where}";
    }
    
    /// <summary>
    /// Проверяет имя SQL-идентификатора и экранирует его.
    /// </summary>
    /// <param name="identifier">
    /// Имя таблицы или столбца.
    /// </param>
    /// <returns>
    /// Проверенный SQL-идентификатор в квадратных скобках.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Имя идентификатора имеет недопустимый формат.
    /// </exception>
    private static string EscapeIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier) ||
            !Regex.IsMatch(
                identifier,
                "^[A-Za-z_][A-Za-z0-9_]*$"))
        {
            throw new ArgumentException(
                "Недопустимое имя SQL-идентификатора.",
                nameof(identifier));
        }
 
        return $"\"{identifier}\"";
    }
    
    private static void CheckNull<T>(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity), 
                "Переданный объект оказался null!");
    }
    
    private static void CheckEmpty<T>(T[] array)
    {
        if (array.Length == 0)
            throw new ArgumentException($"Длина {nameof(array)} = 0! Ошибка!");
    }
}