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
            .Where(x => x.ConversationId == conversationId)
            .Select(x => x.User);
    }

    public async Task<int> CreateGroupChatAsync(List<int> userIds, string groupTitle, int curUser, int fileId = 4)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        var conversation = new ConversationEntity() { IsGroupChat = true };
        await AddAsync(conversation);
        userIds.Add(curUser);

        await AddEntriesGroupChatAsync(conversation.Id, userIds, groupTitle, fileId);
        var firstMessage = new MessageEntity()
            { Content = $"Беседа {groupTitle} создана", ConversationId = conversation.Id, UserId = curUser };
        dbContext.Messages.Add(firstMessage);
        await dbContext.SaveChangesAsync();
        conversation.LastMessageId = firstMessage.Id;
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return conversation.Id;
    }

    private async Task AddEntriesGroupChatAsync(int conversationId, IEnumerable<int> usersId, string title, int fileId = 4)
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

    public async Task<List<ConversationModel>> GetUsersLastConservationsDataAsync(int userId)
    {
        var userConversations = dbContext.UsersConversation
            .Where(x => x.UserId == userId)
            .Join(dbContext.Conversations, x => x.ConversationId, x => x.Id,
                (x, y) => new
                {
                    x.ConversationId, y.LastMessageId, LastMessageSeenId = x.LastMessageSeenId, x.PictureId, x.Title
                })
            .Join(dbContext.Messages, x => x.LastMessageId, x => x.Id,
                (x, y) => new ConversationModel()
                {
                    ConversationId = x.ConversationId, LastMessageContent = y.Content,
                    LastMessageTime = y.CreateTime.ToLocalTime(),
                    ConversationTitle = x.Title, ConversationPictureId = x.PictureId,
                    HasNewMessages = y.Id != x.LastMessageSeenId
                })
            .OrderByDescending(x => x.LastMessageTime)
            .ToListAsync();

        return await userConversations;
    }

    public async Task<PersonalConversationsView?> GetByUsersIdAsync(int id1, int id2)
    {
        return await dbContext.PersonalConversationsView
            .FirstOrDefaultAsync(x => x.uid1 == id1 && x.uid2 == id2 || x.uid2 == id1 && x.uid1 == id2);
    }

    public async Task<PersonalConversationsView?> GetByConversationId(int conversationId)
    {
        return await dbContext.PersonalConversationsView.FirstOrDefaultAsync(x => x.conversationid == conversationId);
    }

    public async Task UpdateLastSeenMessageAsync(int conversationId, int userId, MessageEntity message)
    {
        var usersConversationEntity =
            await dbContext.UsersConversation.FirstOrDefaultAsync(x =>
                x.ConversationId == conversationId && x.UserId == userId);
        usersConversationEntity.LastMessageSeenId = message.Id;
        await dbContext.SaveChangesAsync();
    }
}