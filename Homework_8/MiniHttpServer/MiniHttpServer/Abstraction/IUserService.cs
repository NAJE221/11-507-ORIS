using MiniHttpServer.Entities;

namespace MiniHttpServer.Abstraction;

public interface IUserService
{
    public Task<IReadOnlyList<Users>> GetUsersAsync();
}