using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Repositories;
using OVCHEGRAM.Extensions;
using OVCHEGRAM.Models;

namespace OVCHEGRAM.Controllers;

[Authorize]
public class MessageController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ConversationRepository _conversationRepository;
    private readonly MessageRepository _messagesRepository;
    private readonly FileRepository _fileRepository;
    private readonly UserRepository _userRepository;

    public MessageController(ILogger<HomeController> logger,
        ConversationRepository conversationsRepository, MessageRepository messagesRepository,
        FileRepository fileRepository, UserRepository userRepository)
    {
        _logger = logger;
        _conversationRepository = conversationsRepository;
        _messagesRepository = messagesRepository;
        _fileRepository = fileRepository;
        _userRepository = userRepository;
    }

    public async Task<IActionResult> Conversation(int conversationId)
    {
        var userId = User.GetUserId();
        var conversation = await _conversationRepository.GetUserConversationAsync(userId, conversationId);
        if (conversation == null)
            return Forbid();
        var conversationPicPath = FileRepository.GetFilePathByName(conversation.File?.FileName);
        var messages = await _messagesRepository.GetLastMessagesAsync(x => x.ConversationId == conversationId);
        if (messages.Count > 0)
            await _conversationRepository.UpdateLastSeenMessageAsync(conversationId, userId, messages[0]);
        var users = await _conversationRepository.GetUsersByConversationAsync(conversationId);
        return View((conversationPicPath, conversation.Title, await Converter.ConvertToMessageModel(messages),
            await Converter.ConvertManyToUserProfile(users.ToList())));
    }

    [HttpPost]
    public async Task SendMessage(int conversationId, string? message, IFormFile file = null, bool isImage = false)
    {
        var messageEntity = new MessageEntity()
            { ConversationId = conversationId, UserId = User.GetUserId() };
        if (!string.IsNullOrEmpty(message)) messageEntity.Content = message;
        if (file != null)
        {
            messageEntity.FileId = await _fileRepository.UploadFileAsync(file, isImage);
        }

        await _messagesRepository.AddAsync(messageEntity);
        var conversationEntity = await _conversationRepository.GetByIdAsync(conversationId);
        conversationEntity.LastMessageId = messageEntity.Id;
        await _conversationRepository.UpdateAsync(conversationEntity);
    }

    [HttpGet]
    public async Task<IActionResult> LoadMessages(int conversationId, int lastMessageId = 0, bool takeOld = false,
        int pageSize = 20)
    {
        var messages = await _messagesRepository.GetLastMessagesAsync(x =>
            x.ConversationId == conversationId && (takeOld ? lastMessageId > x.Id : lastMessageId < x.Id), pageSize);
        if (!takeOld && messages.Count > 0)
            await _conversationRepository.UpdateLastSeenMessageAsync(conversationId, User.GetUserId(), messages[0]);
        return PartialView("_PartialMessages", await Converter.ConvertToMessageModel(messages));
    }
}