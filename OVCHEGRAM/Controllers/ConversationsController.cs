using Microsoft.AspNetCore.Mvc;
using OVCHEGRAM.Models;
using OVCHEGRAM.Services;

namespace OVCHEGRAM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConversationsController : Controller
{
    private readonly ConversationService _conversationService;
    private readonly MessageService _messageService;

    public ConversationsController(ConversationService conversationService, MessageService messageService)
    {
        _conversationService = conversationService;
        _messageService = messageService;
    }

    [HttpGet("{id:int}", Name = nameof(GetConversationById))]
    public async Task<ActionResult<ConversationDto>> GetConversationById(int id)
    {
        var users = await _conversationService.GetUsers(id);
        if (users == null || users.Count == 0) return NotFound();
        var dto = new ConversationDto() { Users = users };
        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> CreateConversation([FromBody] GroupChatdto dto)
    {
        int conversationId;
        switch (dto.UserIds.Count)
        {
            case (< 2):
                ModelState.AddModelError("userids", "Conversation must contains at least 2 users");
                return UnprocessableEntity(ModelState);
            case 2:
                conversationId =
                    (int)await _conversationService.GetOrCreatePersonalConversation(dto.UserIds[0], dto.UserIds[1]);
                break;
            default:
                conversationId =
                    await _conversationService.CreateGroupChat(dto);
                break;
        }

        return CreatedAtRoute(nameof(GetConversationById), new { id = conversationId }, conversationId);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<List<MessageModel>>> GetMessages([FromRoute] int conversationId,
        [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var messages = await _messageService.GetMessages(conversationId, pageNumber, pageSize);
        return Ok(messages);
    }
}