using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;

namespace OVCHEGRAM.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserRepository _userRepository;
    private readonly FileRepository _fileRepository;

    public AuthController(ILogger<AuthController> logger, UserRepository userRepository, FileRepository fileRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _fileRepository = fileRepository;
    }

    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registration(RegistrationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _userRepository.GetByNickNameAsync(model.Nickname) != null)
        {
            ModelState.AddModelError("Nickname", "Этот никнейм уже занят");
            return View(model);
        }

        var userEntry = new UserEntity()
        {
            FirstName = model.FirstName, SecondName = model.SecondName, Gender = model.Gender,
            Nickname = model.Nickname, Password = model.Password, Town = model.Town,
            Messages = new List<MessageEntity>(),
            UsersConversations = new List<UsersConversationEntity>()
        };
        if (model.File != null)
        {
            var fileId = _fileRepository.UploadFileAsync(model.File);
            userEntry.ProfilePicId = await fileId;
        }
        await _userRepository.AddAsync(userEntry);
        GetClaimsPrincipal(model, userEntry.Id);
        return RedirectToAction("Profile","ME", new {id = userEntry.Id});
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userRepository.GetByNickNameAsync(model.Nickname);
        if (user == null || user.Password != model.Password)
        {
            ModelState.AddModelError("LoginError", "Неверный логин или пароль");
            return View(model);
        }

        GetClaimsPrincipal(model, user.Id);
        return RedirectToRoute(new { controller = "ME", action = "Profile", id = user.Id });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    private async void GetClaimsPrincipal(IAuthModel model, int id)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Nickname),
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.StayLogIn,
            ExpiresUtc = model.StayLogIn ? DateTimeOffset.UtcNow.AddDays(30) : null
        };
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }
}