namespace OVCHEGRAM.DBModels;

public class FileEntity
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public bool isImage { get; set; }
    public List<MessageEntity> Messages { get; set; }
    public List<UserEntity> Users { get; set; }
    public List<UsersConversationEntity> UsersConversations { get; set; }
}