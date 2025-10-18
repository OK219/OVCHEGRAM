using System.Linq.Expressions;
using AutoMapper;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;

namespace OVCHEGRAM.Services;

public class MessageService
{
    private readonly MessageRepository _messageRepository;
    private readonly ConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly FileRepository _fileRepository;

    public MessageService(MessageRepository messageRepository, ConversationRepository conversationRepository,
        IMapper mapper, FileRepository fileRepository)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _mapper = mapper;
        _fileRepository = fileRepository;
    }

    public async Task<List<MessageModel>> GetLastMessages(int conversationId, Expression<Func<MessageEntity, bool>> filter = null, int count = 20)
    {
        filter ??= x => true;
        var messages =
            await _messageRepository.GetLastMessagesAsync(conversationId, filter, count);
        return messages
            .Select(_mapper.Map<MessageModel>)
            .ToList();
    }

    public async Task CreateMessage(int conversationId, int userId, string? message, IFormFile file = null,
        bool isImage = false)
    {
        var messageEntity = new MessageEntity()
            { ConversationId = conversationId, UserId = userId };
        if (!string.IsNullOrEmpty(message)) messageEntity.Content = message;
        if (file != null)
        {
            messageEntity.FileId = await _fileRepository.UploadFileAsync(file, isImage);
        }

        await _messageRepository.AddAsync(messageEntity);
        var conversationEntity = await _conversationRepository.GetByIdAsync(conversationId);
        conversationEntity.LastMessageId = messageEntity.Id;
        await _conversationRepository.UpdateAsync(conversationEntity);
    }

    public async Task<List<MessageModel>> GetMessages(int conversationId, int pageNum = 1, int pageSize = 10)
    {
        var messages = await _messageRepository.GetMessagesByConversationId(conversationId, pageNum, pageSize);
        return messages
            .Select(_mapper.Map<MessageModel>)
            .ToList();
    }
}