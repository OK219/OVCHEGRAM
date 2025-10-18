using System.ComponentModel.DataAnnotations;

namespace OVCHEGRAM.Models;

public class UserPartialDto : IUserDto
{
    [Required] public string FirstName { get; set; }
    [Required] public string SecondName { get; set; }
    [Required] public int Id { get; set; }
    public string FilePath { get; set; }
}