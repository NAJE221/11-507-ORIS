using MiniHttpServer.ORM.Attributes;

namespace MiniHttpServer.Entities;

[TableName("users")]
public class Users
{
    [PrimaryKey]
    [ColumnName("id")]
    int Id { get; set; }
    
    [ColumnName("name")]
    string Name { get; set; }
    
    [ColumnName("age")]
    int Age { get; set; }
}


