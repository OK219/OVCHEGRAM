using System.ComponentModel.DataAnnotations;

namespace OVCHEGRAM.Models;

public class UserUpdateDto : IUserDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string SecondName { get; set; }
    [Required]
    public string Town { get; set; }
    [Required]
    public Gender? Gender { get; set; }
    public IFormFile? ProfilePicture { get; set; }
}