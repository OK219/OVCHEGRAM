using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Extensions;
using OVCHEGRAM.Models;
using OVCHEGRAM.Services;

namespace OVCHEGRAM.Controllers;

[Authorize]
[Route("/[controller]")]
public class ChatController : Controller
{
    private readonly ConversationService _conversationService;
    private readonly UserService _userService;
    private readonly MessageService _messageService;

    public ChatController(ConversationService conversationService, UserService userService,
        MessageService messageService)
    {
        _conversationService = conversationService;
        _userService = userService;
        _messageService = messageService;
    }

    [HttpGet("conversations")]
    public async Task<IActionResult> Conversations()
    {
        var conversations = await _conversationService.GetUsersLastConservationsDataAsync(User.GetUserId());
        foreach (var conversation in conversations)
        {
            conversation.LastMessageContent ??= "File";
            if (conversation.LastMessageContent.Length > 30)
                conversation.LastMessageContent = conversation.LastMessageContent[..30] + "...";
        }

        return View(conversations);
    }

    [HttpPost("createGroupChat")]
    public async Task<IActionResult> CreateGroupChat(string groupTitle, IFormFile file, List<int> userIds)
    {
        var groupChat = new GroupChatdto() { GroupTitle = groupTitle, File = file, UserIds = userIds };
        groupChat.UserIds?.Add(User.GetUserId());
        var conversationId = await _conversationService.CreateGroupChat(groupChat);
        return Ok(new { redirectUrl = Url.Action("Conversation", "Chat", new { conversationId }) });
    }

    [HttpPost("personalConversation")]
    public async Task<IActionResult> RedirectToPersonalConversation([FromQuery] int id1)
    {
        var userId = User.GetUserId();
        var conversationId = await _conversationService.GetOrCreatePersonalConversation(id1, userId);
        return RedirectToAction("Conversation", "Chat", new { conversationId = conversationId });
    }

    [HttpGet("{conversationId}")]
    public async Task<IActionResult> Conversation([FromRoute] int conversationId)
    {
        var userId = User.GetUserId();
        var conversation = await _conversationService.GetConversationByUserAndId(userId, conversationId);
        if (conversation == null)
            return Forbid();
        var messages = await _messageService.GetLastMessages(conversationId);
        if (messages.Count > 0)
            await _conversationService.UpdateLastSeenMessageAsync(conversationId, userId, messages[0].Id);
        var users = await _userService.GetUsersFromConversation<UserPartialDto>(conversationId);
        return View((conversation.PicturePath, conversation.Title, messages, users));
    }

    [HttpPost("sendMessage")]
    public async Task SendMessage(int conversationId, string? message, IFormFile file = null, bool isImage = false)
    {
        await _messageService.CreateMessage(conversationId, User.GetUserId(), message, file, isImage);
    }

    [HttpGet("loadMessages/{conversationId:int}")]
    public async Task<IActionResult> LoadMessages([FromRoute] int conversationId, [FromQuery] int lastMessageId = 0,
        [FromQuery] bool takeOld = false,
        int pageSize = 20)
    {
        var messages = await _messageService.GetLastMessages(conversationId,
            x => (takeOld ? lastMessageId > x.Id : lastMessageId < x.Id), pageSize);
        if (!takeOld && messages.Count > 0)
            await _conversationService.UpdateLastSeenMessageAsync(conversationId, User.GetUserId(), messages[0].Id);
        return PartialView("_PartialMessages", messages);
    }
}