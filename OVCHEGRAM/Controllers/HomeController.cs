using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OVCHEGRAM.Models;
using OVCHEGRAM.Extensions;

namespace OVCHEGRAM.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        if (!User.Identity.IsAuthenticated) return View();
        var id = User.GetUserId();
        return RedirectToAction("Profile", "ME", new { id });

    }

    public IActionResult ZUEV()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}