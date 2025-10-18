using System.ComponentModel.DataAnnotations;

namespace OVCHEGRAM.Models;

public class LoginViewModel
{
    public string Nickname { get; set; }
    public string Password { get; set; }
    public bool StayLogIn { get; set; }
}