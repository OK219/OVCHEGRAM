using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;

namespace OVCHEGRAM;

public static class Converter
{
    public static async Task<IEnumerable<MessageModel>> ConvertToMessageModel(IEnumerable<MessageEntity> messages)
    {
        return messages.Select(x => new MessageModel()
        {
            Content = x.Content,
            FilePath = x.FileId != null ? FileRepository.GetFilePathByName(x.File.FileName) : "",
            UserId = x.UserId, Id = x.Id, isImage = x.FileId != null && x.File.isImage, SendTime = x.CreateTime,
            SenderName = x.User.FirstName + " " + x.User.SecondName,
            ProfilePicPath = FileRepository.GetFilePathByName(x.User.ProfilePic?.FileName)
        });
    }
    
    public static async Task<UserProfileModel> ConvertToProfileModel(UserEntity? user)
    {
        return new UserProfileModel()
        {
            Gender = user.Gender, FirstName = user.FirstName, SecondName = user.SecondName,
            Town = user.Town, Id = user.Id,
            FilePath = user.ProfilePicId != null
                ? FileRepository.GetFilePathByName(user.ProfilePic.FileName)
                : FileRepository.GetFilePathByName()
        };
    }

    public static async Task<IEnumerable<UserProfileModel>> ConvertManyToUserProfile(IEnumerable<UserEntity?> users)
    {
        var tasks = users.Select(ConvertToProfileModel);
        return await Task.WhenAll(tasks);;
    }
}