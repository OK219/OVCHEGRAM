using Microsoft.EntityFrameworkCore;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;

namespace OVCHEGRAM.Repositories;

public class ConversationRepository(OvchegramDbContext dbContext) : BaseRepository<ConversationEntity>(dbContext)
{
    public async Task<ConversationEntity?> GetByIdAsync(int id)
    {
        return await dbContext.Conversations
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<UsersConversationEntity?> GetUserConversationAsync(int userId, int conversationId)
    {
        return await dbContext.UsersConversation
            .Include(x => x.File)
            .FirstOrDefaultAsync(x =>
                x.ConversationId == conversationId && x.UserId == userId);
    }

    public async Task<IQueryable<UserEntity?>> GetUsersByConversationAsync(int conversationId)
    {
        return dbContext.UsersConversation
            .Include(x => x.User)
            .Include(x => x.User.ProfilePic)
            .Where(x => x.ConversationId == conversationId)
            .Select(x => x.User);
    }

    public async Task<int> CreateGroupChatAsync(List<int> userIds, string groupTitle, int? fileId)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        var conversation = new ConversationEntity() { IsGroupChat = true };
        await AddAsync(conversation);
        await AddEntriesGroupChatAsync(conversation.Id, userIds, groupTitle, fileId);
        var firstMessage = new MessageEntity()
            { Content = $"Беседа {groupTitle} создана", ConversationId = conversation.Id, UserId = userIds.Last() };
        dbContext.Messages.Add(firstMessage);
        await dbContext.SaveChangesAsync();
        conversation.LastMessageId = firstMessage.Id;
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return conversation.Id;
    }

    private async Task AddEntriesGroupChatAsync(int conversationId, IEnumerable<int> usersId, string title,
        int? fileId)
    {
        IEnumerable<UsersConversationEntity?> entries = usersId.Select(userId => new UsersConversationEntity
        {
            ConversationId = conversationId,
            UserId = userId,
            Title = title,
            PictureId = fileId
        });

        await dbContext.UsersConversation.AddRangeAsync(entries);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddEntriesPersonalChatAsync(int conversationId, int firstUserId, int secondUserId)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        var users = await dbContext.Users.Where(x => new[] { firstUserId, secondUserId }.Contains(x.Id)).ToListAsync();
        await dbContext.UsersConversation.AddAsync(new UsersConversationEntity()
        {
            ConversationId = conversationId, UserId = users[0].Id,
            Title = users[1].FirstName + " " + users[1].SecondName,
            PictureId = users[1].ProfilePicId
        });
        await dbContext.UsersConversation.AddAsync(new UsersConversationEntity()
        {
            ConversationId = conversationId, UserId = users[1].Id,
            Title = users[0].FirstName + " " + users[0].SecondName,
            PictureId = users[0].ProfilePicId
        });
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<int?> GetPersonalConversationId(int id1, int id2)
    {
        var conversations = dbContext.UsersConversation.Join(dbContext.UsersConversation, x => x.ConversationId,
            y => y.ConversationId, (x, y) => new { firstId = x.UserId, secondId = y.UserId, x.ConversationId });
        var conversation = await conversations
            .FirstOrDefaultAsync(x => x.firstId == id1 && x.secondId == id2 || x.secondId == id1 && x.firstId == id2);
        return conversation?.ConversationId;
    }

    public async Task UpdateLastSeenMessageAsync(int conversationId, int userId, int messageId)
    {
        var usersConversationEntity =
            await dbContext.UsersConversation.FirstOrDefaultAsync(x =>
                x.ConversationId == conversationId && x.UserId == userId);
        usersConversationEntity.LastMessageSeenId = messageId;
        await dbContext.SaveChangesAsync();
    }

    public List<ConversationEntity> GetConversationsByUserId(int userId)
    {
        var conversationIds = dbContext.UsersConversation
            .Where(x => x.UserId == userId)
            .Select(x => x.ConversationId);
        return dbContext.Conversations
            .Where(x => conversationIds.Contains(x.Id))
            .ToList();
    }

    public List<UsersConversationEntity> GetUsersConversations(int userId)
    {
        return dbContext.UsersConversation
            .Where(x => x.UserId == userId)
            .ToList();
    }

    public async Task UpdateUsersConversation(int userId)
    {
        var user = dbContext.Users
            .FirstOrDefault(x => x.Id == userId);
        var conversationEntities = GetConversationsByUserId(userId)
            .Where(x => !x.IsGroupChat)
            .Select(x => x.Id);
        var userConversationEntities = dbContext.UsersConversation
            .Where(x => conversationEntities.Contains(x.ConversationId) && x.UserId != userId);
        foreach (var entity in userConversationEntities)
        {
            entity.Title = user.FirstName + " " + user.SecondName;
            entity.PictureId = user.ProfilePicId;
        }

        await dbContext.SaveChangesAsync();
    }
}