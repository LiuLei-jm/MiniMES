using MiniMES.Domain.Entities;
using MiniMES.Domain.Interfaces;
using MiniMES.Infrastructure.Persistence;

namespace MiniMES.Infrastructure.Repositories;

[Obsolete("Please user IApplicationDbContext instead")]
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<User?> GetByIdAsync(Guid id) => await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}