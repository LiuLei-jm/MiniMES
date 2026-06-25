using MiniMES.Domain.Entities;

namespace MiniMES.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUserNameAsync(string userName);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}
