using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OVCHEGRAM.Models;
using OVCHEGRAM.Extensions;
using OVCHEGRAM.Services;

namespace OVCHEGRAM.Controllers;

[Authorize]
[Route("/[controller]")]
public class ProfileController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserService _userService;


    public ProfileController(ILogger<HomeController> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Profile([FromRoute] int id)
    {
        var user = await _userService.GetUser<UserUpdateDto>(id);
        if (user == null) return Forbid();
        var profilePicPath = await _userService.GetUserProfilePath(id);
        var isAllowed = id == User.GetUserId();
        return View((user, isAllowed, profilePicPath));
    }

    [HttpGet("users")]
    public async Task<IActionResult> Users()
    {
        var users = await _userService.GetPartialUsers();
        users = users.Where(x => x.Id != User.GetUserId()).ToList();
        return View(users);
    }

    [HttpPost("updateProfile")]
    public async Task<IActionResult> UpdateProfile([FromForm] UserUpdateDto newUserProfile)
    {
        var userId = User.GetUserId();
        await _userService.UpdateUser(userId, newUserProfile);
        return RedirectToAction("Profile", new { id = userId });
    }

    [HttpGet("usersHtml")]
    public async Task<IActionResult> GetUsersHtml([FromQuery]int page = 1, [FromQuery] string filter = "", int pageSize = 10)
    {
        var users = await GetUsers(page, filter, pageSize);
        return PartialView("_PartialUsers", users);
    }

    [HttpGet("usersNamesHtml")]
    public async Task<IActionResult> GetUsersNamesHtml(int page = 1, string filter = "", int pageSize = 10)
    {
        var users = await GetUsers(page, filter, pageSize);
        return PartialView("_PartialUsersNames", users);
    }

    private async Task<List<UserPartialDto>> GetUsers(int page = 1, string filter = "", int pageSize = 10)
    {
        filter = filter.ToLower();
        var userId = User.GetUserId();
        var users = await _userService.GetPartialUsers(page, pageSize);
        return users.Where(x => (string.IsNullOrEmpty(filter) ||
                                (x.FirstName + " " + x.SecondName).StartsWith(filter,
                                    StringComparison.CurrentCultureIgnoreCase) ||
                                (x.SecondName + " " + x.FirstName).StartsWith(filter,
                                    StringComparison.CurrentCultureIgnoreCase)) &&
                                x.Id != userId)
            .ToList();
    }

    [HttpGet("logOut")]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToRoute(new { controller = "Home", action = "Index" });
    }
}