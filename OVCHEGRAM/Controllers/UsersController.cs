using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OVCHEGRAM.Extensions;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;
using OVCHEGRAM.Services;

namespace OVCHEGRAM.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserService _userService;

    public UsersController(ILogger<HomeController> logger, UserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpGet("{id:int}", Name = nameof(GetUserById))]
    public async Task<ActionResult<UserDto>> GetUserById([FromRoute] int id)
    {
        var userDto = await _userService.GetUser<UserDto>(id);
        if (userDto == null) return NotFound();
        return Ok(userDto);
    }

    [HttpGet("")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var userDtos = await _userService.GetUsers<UserDto>(pageNumber, pageSize);
        return Ok(userDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        var userId = await _userService.CreateUser(dto);
        return CreatedAtRoute(nameof(GetUserById), new { id = userId }, userId);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UserUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);
        await _userService.UpdateUser(id, dto);

        return NoContent();
    }
}