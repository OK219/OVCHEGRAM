namespace OVCHEGRAM.DBModels;

public class MessageEntity
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public ConversationEntity Conversation { get; set; }
    public int UserId { get; set; }
    public UserEntity User { get; set; }
    public DateTime CreateTime { get; set; }
    public int? FileId { get; set; }
    public FileEntity File { get; set; }
    public List<UsersConversationEntity> UsersConversation { get; set; }
    public string? Content { get; set; }
}