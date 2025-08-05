namespace OVCHEGRAM.Models;

public class MessageModel
{
    public string Content { get; set; }
    public string SenderName { get; set; }
    public string ProfilePicPath { get; set; }
    public int Id { get; set; }
    public string FilePath { get; set; }
    public bool isImage { get; set; }
    public int UserId { get; set; }
    public DateTime SendTime { get; set; }
}