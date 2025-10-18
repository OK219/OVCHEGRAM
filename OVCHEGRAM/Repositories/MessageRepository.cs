using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;

namespace OVCHEGRAM.Repositories;

public class MessageRepository(OvchegramDbContext dbContext) : BaseRepository<MessageEntity>(dbContext)
{
    public async Task<List<MessageEntity>> GetLastMessagesAsync(int conversationId,
        Expression<Func<MessageEntity, bool>> filter = null, int count = 15)
    {
        filter ??= x => true;
        return await dbContext.Messages
            .Include(x => x.File)
            .Include(x => x.User)
            .Include(x => x.User.ProfilePic)
            .OrderByDescending(x => x.SendTime)
            .Where(x => x.ConversationId == conversationId)
            .Where(filter)
            .Take(count)
            .ToListAsync();
    }

    public async Task<MessageEntity?> GetLastMessageByConversationId(int conversationId)
    {
        return dbContext
            .Messages
            .OrderBy(x => x.SendTime)
            .LastOrDefault(x => x.ConversationId == conversationId);
    }

    public async Task<List<MessageEntity>> GetMessagesByConversationId(int conversationId, int pageNum = 1,
        int pageSize = 10)
    {
        return await dbContext
            .Messages
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}