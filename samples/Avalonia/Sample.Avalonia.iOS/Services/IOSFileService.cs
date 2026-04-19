using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using Avalonia.Platform;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Sample.Avalonia.iOS.Services;

// ReSharper disable once InconsistentNaming
public class IOSFileService : IFileService
{
    private readonly ILogger<IFileService> _logger;

    public IOSFileService(ILogger<IFileService> logger)
    {
        _logger = logger;
    }

    public async Task<string> GetFileAsync(string folderName, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            var uri = new Uri($"avares://ADaxer.MvvmNav.Sample.Avalonia.iOS/Assets/{folderName.ToLowerInvariant()}/{fileName}");

            await using var stream = AssetLoader.Open(uri);
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading file {Folder}/{File}", folderName, fileName);
            return "# Ja mei";
        }
    }}
