namespace MiniHttpServer.ORM.Attributes;

public class TableNameAttribute(string tableName) : Attribute
{
    public readonly string TableName = tableName 
                                        ?? throw new ArgumentNullException(nameof(tableName));
}