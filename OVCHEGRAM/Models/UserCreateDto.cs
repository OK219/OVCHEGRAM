using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OVCHEGRAM.Models;

public class UserCreateDto : IUserDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string SecondName { get; set; }
    [Required]
    public string Nickname { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Town { get; set; }
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Gender? Gender { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}