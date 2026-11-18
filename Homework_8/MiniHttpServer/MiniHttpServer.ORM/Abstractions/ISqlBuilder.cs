namespace MiniHttpServer.ORM.Abstractions;

public interface ISqlBuilder
{
    string BuildSelect(string tableName);
    string BuildSelectById<T>(string tableName) where T : class;
    string BuildInsert<T>(string tableName, T entity) where T : class;
    string BuildUpdate<T>(string tableName, T entity) where T : class;
    string BuildDelete<T>(string tableName) where T : class;
}