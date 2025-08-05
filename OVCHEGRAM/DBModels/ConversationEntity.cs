namespace OVCHEGRAM.DBModels;

public class ConversationEntity
{
    public int Id { get; set; }
    public int LastMessageId { get; set; }
    public bool IsGroupChat { get; set; }
    public List<MessageEntity> Messages { get; set; }
    public List<UsersConversationEntity> UsersConversations { get; set; }
}