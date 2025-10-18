using AutoMapper;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;

namespace OVCHEGRAM.Services;

public class ConversationService(
    ConversationRepository conversationRepository,
    MessageRepository messageRepository,
    FileRepository fileRepository,
    IMapper mapper)
{
    private readonly ConversationRepository _conversationRepository = conversationRepository;
    private readonly MessageRepository _messageRepository = messageRepository;
    private readonly FileRepository _fileRepository = fileRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<List<ConversationModel>> GetUsersLastConservationsDataAsync(int userId)
    {
        var usersConversations = _conversationRepository.GetUsersConversations(userId);
        var conversations = new List<ConversationModel>();
        foreach (var conversation in usersConversations)
        {
            var lastMessage = await _messageRepository.GetLastMessageByConversationId(conversation.ConversationId);
            var filePath = await _fileRepository.GetFilePathByIdAsync(conversation.PictureId);
            var conversationModel = new ConversationModel()
            {
                ConversationId = conversation.ConversationId,
                Title = conversation.Title, PicturePath = filePath
            };
            if (lastMessage != null)
            {
                conversationModel.LastMessageContent = lastMessage.Content;
                conversationModel.LastMessageTime = lastMessage.SendTime.ToLocalTime();
                conversationModel.HasNewMessages = lastMessage.Id != conversation.LastMessageSeenId;
            }

            conversations.Add(conversationModel);
        }

        return conversations
            .OrderByDescending(x => x.LastMessageTime)
            .ToList();
    }

    public async Task<int> CreateGroupChat(GroupChatdto groupChat)
    {
        int? fileId = null;
        if (groupChat.File != null)
            fileId = await _fileRepository.UploadFileAsync(groupChat.File);
        var conversationId =
            await _conversationRepository.CreateGroupChatAsync(groupChat.UserIds, groupChat.GroupTitle,
                fileId);
        return conversationId;
    }

    public async Task<int?> GetOrCreatePersonalConversation(int id1, int id2)
    {
        var conversationId = await _conversationRepository.GetPersonalConversationId(id1, id2);
        if (conversationId != null)
            return conversationId;

        var conversationEntry = new ConversationEntity();
        await _conversationRepository.AddAsync(conversationEntry);
        await _conversationRepository.AddEntriesPersonalChatAsync(conversationEntry.Id, id1, id2);
        return conversationEntry.Id;
    }

    public async Task<ConversationPartialDto?> GetConversationByUserAndId(int userId, int conversationId)
    {
        var conversation = await _conversationRepository.GetUserConversationAsync(userId, conversationId);
        return mapper.Map<ConversationPartialDto>(conversation);
    }

    public async Task UpdateLastSeenMessageAsync(int conversationId, int userId, int messageId)
    {
        await _conversationRepository.UpdateLastSeenMessageAsync(conversationId, userId, messageId);
    }

    public async Task<List<UserDto>> GetUsers(int conversationId)
    {
        var users = await _conversationRepository.GetUsersByConversationAsync(conversationId);
        return users.AsEnumerable().Select(_mapper.Map<UserDto>).ToList();
    }
}