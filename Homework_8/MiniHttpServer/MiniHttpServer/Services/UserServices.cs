using MiniHttpServer.Abstraction;
using MiniHttpServer.Entities;
using MiniHttpServer.ORM.Abstractions;

namespace MiniHttpServer.Services;

public class UserServices(IOrmContext context) : IUserService
{
    public async Task<IReadOnlyList<Users>> GetUsersAsync()
    {
        return await context.GetAllAsync<Users>();
    }
}