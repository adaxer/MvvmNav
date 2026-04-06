using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace ADaxer.MvvmNav.Sample.Common.Services;

public class FileService : IFileService
{
    public async Task<string> GetFileAsync(string path, CancellationToken cancellationToken=default)
    {
        return await File.ReadAllTextAsync(path, cancellationToken);
    }
}
