using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;
using OVCHEGRAM.Services;

namespace OVCHEGRAM.Controllers;

[Route("/[controller]")]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserRepository _userRepository;
    private readonly UserService _userService;
    private readonly IMapper _mapper;

    public AuthController(ILogger<AuthController> logger, UserRepository userRepository,
        UserService userService, IMapper mapper)
    {
        _logger = logger;
        _userRepository = userRepository;
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet("/[action]")]
    public IActionResult Registration()
    {
        _logger.LogInformation("Registration process started");
        return View();
    }

    [HttpPost("/[action]")]
    public async Task<IActionResult> Registration([FromForm] RegistrationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Registration model invalid");
            return View(model);
        }

        if (await _userRepository.GetByNickNameAsync(model.Nickname) != null)
        {
            ModelState.AddModelError("Nickname", "Этот никнейм уже занят");
            return View(model);
        }

        var userCreateDto = _mapper.Map<UserCreateDto>(model);
        var userId = await _userService.CreateUser(userCreateDto);

        GetClaimsPrincipal(userId, model.Nickname, model.StayLogIn);
        return RedirectToAction("Profile", "Profile", new { id = userId });
    }

    [HttpGet("/[action]")]
    public IActionResult Login()
    {
        _logger.LogInformation("Login process started");
        return View();
    }

    [HttpPost("/[action]")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Login model invalid");
            return View(model);
        }


        var user = await _userRepository.GetByNickNameAsync(model.Nickname);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            _logger.LogWarning("User not exist");
            ModelState.AddModelError("LoginError", "Неверный логин или пароль");
            return View(model);
        }

        GetClaimsPrincipal(user.Id, model.Nickname, model.StayLogIn);
        return RedirectToRoute(new { controller = "Profile", action = "Profile", id = user.Id });
    }

    private async void GetClaimsPrincipal(int id, string nickname, bool stayLogIn)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, nickname),
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = stayLogIn,
            ExpiresUtc = stayLogIn ? DateTimeOffset.UtcNow.AddDays(30) : null
        };
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }
}