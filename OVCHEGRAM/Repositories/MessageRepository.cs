using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;

namespace OVCHEGRAM.Repositories;

public class MessageRepository(OvchegramDbContext dbContext) : BaseRepository<MessageEntity>(dbContext)
{
    public async Task<List<MessageEntity>> GetLastMessagesAsync(Expression<Func<MessageEntity, bool>> filter = null, int count = 15)
    {
        filter ??= x => true;
        return await dbContext.Messages
            .Include(x => x.File)
            .Include(x => x.User)
            .Include(x => x.User.ProfilePic)
            .OrderByDescending(x => x.CreateTime)
            .Where(filter)
            .Take(count)
            .ToListAsync();
    }
}