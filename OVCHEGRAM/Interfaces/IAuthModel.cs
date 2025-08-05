using System.ComponentModel.DataAnnotations;

namespace OVCHEGRAM.Models;

public interface IAuthModel
{
    [Required(ErrorMessage = "Укажите логин")]
    public string Nickname { get; set; }
    [Required(ErrorMessage = "Укажите пароль")]
    [MinLength(8, ErrorMessage = "Слишком короткий")]
    public string Password { get; set; }
    public bool StayLogIn { get; set; }
}