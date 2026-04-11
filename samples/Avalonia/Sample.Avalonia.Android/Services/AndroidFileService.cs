using Android.Content.Res;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Sample.Avalonia.Android.Services;

public class AndroidFileService : IFileService
{
    private readonly AssetManager _assets;
    private readonly ILogger<IFileService> _logger;

    public AndroidFileService(ILogger<IFileService> logger)
    {
        _assets = global::Android.App.Application.Context.Assets;
        _logger = logger;
    }

    public async Task<string> GetFileAsync(string path, CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = _assets.Open(path);
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
