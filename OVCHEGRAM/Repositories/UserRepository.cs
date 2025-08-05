using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;

namespace OVCHEGRAM.Repositories;

public class UserRepository(OvchegramDbContext dbContext) : BaseRepository<UserEntity>(dbContext)
{
    private readonly OvchegramDbContext _dbContext = dbContext;

    public async Task<UserEntity?> GetByIdAsync(int id)
    {
        return await _dbContext.Users.Include(x => x.ProfilePic).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<UserEntity?> GetByNickNameAsync(string nickname)
    {
        return await _dbContext.Users.Include(x => x.ProfilePic).FirstOrDefaultAsync(x => x.Nickname == nickname);
    }

    public async Task<List<UserEntity>> GetPageAsync(int page = 1, int pageSize = 10,
        Func<UserEntity, bool>? filter = null)
    {
        filter ??= x => true;
        return dbContext
            .Users.Include(x => x.ProfilePic)
            .Where(filter)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }
}