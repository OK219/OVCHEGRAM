using OVCHEGRAM.Models;

namespace OVCHEGRAM.DBModels;

public class UserEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public string Town { get; set; }
    public Gender? Gender { get; set; }
    public int ProfilePicId { get; set; }
    public FileEntity ProfilePic { get; set; }
    public List<MessageEntity> Messages;
    public List<UsersConversationEntity> UsersConversations { get; set; }
}