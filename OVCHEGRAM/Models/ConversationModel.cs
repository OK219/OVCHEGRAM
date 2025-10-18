namespace OVCHEGRAM.Models;

public class ConversationModel
{
    public int ConversationId { get; set; }
    public string Title { get; set; }
    public string? PicturePath { get; set; }
    public bool HasNewMessages { get; set; }
    public string? LastMessageContent { get; set; }
    public DateTime LastMessageTime { get; set; }
}