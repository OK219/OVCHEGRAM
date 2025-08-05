using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;
using OVCHEGRAM.Extensions;

namespace OVCHEGRAM.Controllers;

[Authorize]
public class MEController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserRepository _userRepository;
    private readonly ConversationRepository _conversationRepository;
    private readonly FileRepository _fileRepository;

    public MEController(ILogger<HomeController> logger, UserRepository userRepository,
        ConversationRepository conversationRepository,
        FileRepository fileRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _conversationRepository = conversationRepository;
        _fileRepository = fileRepository;
    }

    public async Task<IActionResult> Profile(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return Forbid();
        var isAllowed = id == User.GetUserId();
        return View((await Converter.ConvertToProfileModel(user), isAllowed));
    }
    
    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _userRepository.GetPageAsync();
        var use = await Task.WhenAll(users.Select(Converter.ConvertToProfileModel));
        return View(use);
    }
    
    [HttpGet]
    public async Task<IActionResult> Conversations()
    {
        var conversations = await _conversationRepository.GetUsersLastConservationsDataAsync(User.GetUserId());
        foreach (var conversation in conversations)
        {
            conversation.LastMessageContent ??= "File";
            if (conversation.LastMessageContent.Length > 30)
                conversation.LastMessageContent = conversation.LastMessageContent[..30] + "...";
            conversation.ConversationPictureName =
                await _fileRepository.GetFilePathByIdAsync(conversation.ConversationPictureId);
        }

        return View(conversations);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeProfile(UserProfileModel newUserProfile)
    {
        var userId = User.GetUserId();
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return Forbid();
        user.FirstName = newUserProfile.FirstName;
        user.SecondName = newUserProfile.SecondName;
        user.Gender = newUserProfile.Gender;
        user.Town = newUserProfile.Town;
        if (newUserProfile.File != null)
        {
            var fileId = _fileRepository.UploadFileAsync(newUserProfile.File);
            user.ProfilePicId = await fileId;
        }

        await _userRepository.UpdateAsync(user);
        return RedirectToAction("Profile", new { id = user.Id });
    }

    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToRoute(new { controller = "Home", action = "Index" });
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroupChat(string groupTitle, IFormFile file, IEnumerable<int> userIds)
    {
        int? fileId = null;
        if (file != null)
            fileId = await _fileRepository.UploadFileAsync(file);
        var conversationId =
            await _conversationRepository.CreateGroupChatAsync(userIds.ToList(), groupTitle,
                User.GetUserId(), fileId);
        return Ok(new { redirectUrl = Url.Action("Conversation", "Message", new { conversationId }) });
    }

    public async Task<IActionResult> GetUsersHtml(int page = 1, string filter = "", int pageSize = 10)
    {
        var nextItemsTasks = await GetUsers(page, filter, pageSize);
        var nextItems = await Task.WhenAll(nextItemsTasks.Select(Converter.ConvertToProfileModel));
        return PartialView("_PartialUsers", nextItems);
    }

    public async Task<IActionResult> GetUsersNamesHtml(int page = 1, string filter = "", int pageSize = 10)
    {
        var nextItemsTasks = await GetUsers(page, filter, pageSize);
        var nextItems = await Task.WhenAll(nextItemsTasks.Select(Converter.ConvertToProfileModel));
        return PartialView("_PartialUsersNames", nextItems);
    }

    public async Task<IActionResult> RedirectToPersonalConversation(int id1)
    {
        var userId = User.GetUserId();
        var conversationId = await _conversationRepository.GetByUsersIdAsync(id1, userId);
        if (conversationId != null)
            return RedirectToAction("Conversation", "Message", new { conversationId = conversationId });

        var conversationEntry = new ConversationEntity();
        await _conversationRepository.AddAsync(conversationEntry);
        await _conversationRepository.AddEntriesPersonalChatAsync(conversationEntry.Id, id1, userId);
        return RedirectToAction("Conversation", "Message", new { conversationId = conversationEntry.Id });
    }
    
    private async Task<List<UserEntity>> GetUsers(int page = 1, string filter = "", int pageSize = 10)
    {
        filter = filter.ToLower();
        return await _userRepository.GetPageAsync(page, pageSize,
            x => string.IsNullOrEmpty(filter) ||
                 (x.FirstName + " " + x.SecondName).StartsWith(filter, StringComparison.CurrentCultureIgnoreCase) ||
                 (x.SecondName + " " + x.FirstName).StartsWith(filter, StringComparison.CurrentCultureIgnoreCase));
    }
}