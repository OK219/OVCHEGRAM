namespace OVCHEGRAM.Models;

public class ConversationModel
{
    public int ConversationId { get; set; }
    public string ConversationTitle { get; set; }
    public int ConversationPictureId { get; set; }
    public string ConversationPictureName { get; set; }
    public bool HasNewMessages { get; set; }
    public string? LastMessageContent { get; set; }
    public DateTime LastMessageTime { get; set; }
}