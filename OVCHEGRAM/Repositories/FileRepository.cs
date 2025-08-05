using Microsoft.EntityFrameworkCore;
using OVCHEGRAM.DBModels;

namespace OVCHEGRAM.Repositories;

public class FileRepository(OvchegramDbContext dbContext, FileManager fileManager)
    : BaseRepository<FileEntity>(dbContext)
{
    public async Task<int> UploadFileAsync(IFormFile file, bool isImage = false)
    {
        var filePath = fileManager.UploadFile(file);
        var fileEntry = new FileEntity() { FileName = await filePath, isImage = isImage};
        await AddAsync(fileEntry);
        return fileEntry.Id;
    }

    public async Task<string> GetFilePathByIdAsync(int? id = null)
    {
        if (id == null) return "/UserUploads/default.png";
        var name = await dbContext.Files.FirstOrDefaultAsync(x => x.Id == id);
        return "/UserUploads/" + name.FileName;
    }
    
    public static string GetFilePathByName(string name = null)
    {
        if (string.IsNullOrEmpty(name)) return "/UserUploads/default.png";
        return "/UserUploads/" + name;
    }
}