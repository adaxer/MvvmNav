using ADaxer.MvvmNav.Sample.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Sample.Maui.Services;

public sealed class MauiFileService : IFileService
{
    private readonly ILogger<IFileService> _logger;

    public MauiFileService(ILogger<IFileService> logger)
    {
        _logger = logger;
    }
    public async Task<string> GetFileAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var stream = await FileSystem.Current.OpenAppPackageFileAsync(path);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file {Path}", path);
            throw;
        }
    }
}
