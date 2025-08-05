using System.ComponentModel.DataAnnotations;

namespace OVCHEGRAM.Models;

public class UserProfileModel
{
    [Required(ErrorMessage = "Введите имя пожалуйста")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Введите фамилию пожалуйста")]
    public string SecondName { get; set; }
    [Required(ErrorMessage = "Где вы живете?")]
    public string Town { get; set; }
    [Required(ErrorMessage = "Укажите ваш пол, пожалуйста")]
    public Gender? Gender { get; set; }
    public string FilePath { get; set; }
    public int Id { get; set; }
    public IFormFile? File { get; set; }
}