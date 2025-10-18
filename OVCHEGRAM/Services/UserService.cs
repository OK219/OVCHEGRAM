using AutoMapper;
using OVCHEGRAM.DBModels;
using OVCHEGRAM.Models;
using OVCHEGRAM.Repositories;

namespace OVCHEGRAM.Services;

public class UserService(
    UserRepository userRepository,
    ConversationRepository conversationRepository,
    FileRepository fileRepository,
    IMapper mapper)
{
    public async Task<TDto?> GetUser<TDto>(int id) where TDto : class, IUserDto
    {
        var user = await userRepository.GetByIdAsync(id);
        return user == null ? null : mapper.Map<TDto>(user);
    }

    public async Task<List<TDto>> GetUsers<TDto>(int pageNumber = 1, int pageSize = 10) where TDto : IUserDto
    {
        var users = await userRepository.GetPageAsync(pageNumber, pageSize);
        return users
            .Select(mapper.Map<TDto>)
            .ToList();
    }

    public async Task<List<UserPartialDto>> GetPartialUsers(int pageNumber = 1, int pageSize = 10)
    {
        var users = await userRepository.GetPageAsync(pageNumber, pageSize);
        var dtos = users.Select(mapper.Map<UserPartialDto>).ToList();
        for (var i = 0; i < users.Count; i++)
        {
            var path = await fileRepository.GetFilePathByIdAsync(users[i].ProfilePicId);
            dtos[i].FilePath = path;
        }

        return dtos;
    }

    public async Task<int> CreateUser(UserCreateDto dto)
    {
        dto.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var userEntity = mapper.Map<UserEntity>(dto);
        if (dto.ProfilePicture != null)
        {
            userEntity.ProfilePicId = await fileRepository.UploadFileAsync(dto.ProfilePicture, true);
        }
        await userRepository.AddAsync(userEntity);
        return userEntity.Id;
    }

    public async Task UpdateUser(int id, UserUpdateDto dto)
    {
        var userEntity = await userRepository.GetByIdAsync(id);
        userEntity = mapper.Map(dto, userEntity);
        if (dto.ProfilePicture != null)
        {
            userEntity.ProfilePicId = await fileRepository.UploadFileAsync(dto.ProfilePicture, true);
        }
        await userRepository.UpdateAsync(userEntity);
        await conversationRepository.UpdateUsersConversation(id);
    }

    public async Task<List<TDto>> GetUsersFromConversation<TDto>(int conversationId) where TDto : IUserDto
    {
        var users = await conversationRepository.GetUsersByConversationAsync(conversationId);
        return users.AsEnumerable().Select(mapper.Map<TDto>).ToList();
    }

    public async Task<string?> GetUserProfilePath(int id)
    {
        return await fileRepository.GetFilePathByIdAsync((await userRepository.GetByIdAsync(id))?.ProfilePicId);
    }
}