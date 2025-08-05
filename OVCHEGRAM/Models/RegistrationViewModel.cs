using System.ComponentModel.DataAnnotations;

namespace OVCHEGRAM.Models;

public class RegistrationViewModel : IAuthModel
{
    [Required(ErrorMessage = "Введите имя пожалуйста")]
    public string FirstName { get; set; }
    [Required(ErrorMessage = "Введите фамилию пожалуйста")]
    public string SecondName { get; set; }
    [Required(ErrorMessage = "Где вы живете?")]
    public string Town { get; set; }
    [Required(ErrorMessage = "Укажите ваш пол, пожалуйста")]
    public Gender? Gender { get; set; }
    [Required(ErrorMessage = "Укажите никнейм, позже вы будете входить с его помощью")]
    public string Nickname { get; set; }
    [Required(ErrorMessage = "Укажите пароль, позже вы будете входить с его помощью")]
    [MinLength(8, ErrorMessage = "Слишком короткий пароль")]
    public string Password { get; set; }
    public bool StayLogIn { get; set; }
    public IFormFile? File { get; set; }
}

public enum Gender
{
    Man,
    Woman
}