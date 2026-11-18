namespace MiniHttpServer.ORM.Attributes;

public class ColumnNameAttribute(string columnName) : Attribute
{
    public readonly string ColumnName = columnName 
                    ?? throw new ArgumentNullException(nameof(columnName));
}