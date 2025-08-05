namespace OVCHEGRAM.DBModels;

public class UsersConversationEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; }
    public int PictureId { get; set; }
    public FileEntity File { get; set; }
    public UserEntity User { get; set; }
    public int ConversationId { get; set; }
    public int? LastMessageSeenId { get; set; }
    public MessageEntity? LastMessageSeen { set; get; }
    public ConversationEntity Conversation { get; set; }
}