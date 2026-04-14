using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace ADaxer.MvvmNav.Sample.Common.Services;

public class FileService : IFileService
{
    public Task<string> GetFileAsync(string folderName, string fileName, CancellationToken cancellationToken = default)
    {
        var basePath = AppContext.BaseDirectory;
        var filePath = Path.Combine(basePath, folderName, fileName);

        return File.ReadAllTextAsync(filePath,  cancellationToken);
    }
}
