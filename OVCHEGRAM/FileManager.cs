using System.Diagnostics.Eventing.Reader;

namespace OVCHEGRAM;

public class FileManager
{
    private readonly string fileDirectory;

    public FileManager()
    {
        var fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserUploads");
        if (!Directory.Exists(fileDirectory))
            Directory.CreateDirectory(fileDirectory);
        this.fileDirectory = fileDirectory;
    }

    public async Task<string> UploadFile(IFormFile file)
    {
        var uniqueFileName = Guid.NewGuid() + "_" + file.FileName;
        var filePath = GetFullPath(uniqueFileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return uniqueFileName;
    }

    public string GetFullPath(string fileName)
    {
        return Path.Combine(fileDirectory, fileName);
    }
}