namespace OVCHEGRAM.Models;

public class GroupChatdto
{
    public string GroupTitle { get; set; }
    public IFormFile? File { get; set; }
    public List<int>? UserIds { get; set; }
}