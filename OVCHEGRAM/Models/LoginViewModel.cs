using System.ComponentModel.DataAnnotations;

namespace OVCHEGRAM.Models;

public class LoginViewModel : IAuthModel
{
    public string Nickname { get; set; }
    public string Password { get; set; }
    public bool StayLogIn { get; set; }
}