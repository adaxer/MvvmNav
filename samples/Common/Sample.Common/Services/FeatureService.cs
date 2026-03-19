using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace ADaxer.MvvmNav.Sample.Common.Services;

public class FeatureService
{
    private readonly IFileService _fileService;
    private List<FeatureItem> _features = [
        new FeatureItem { Name = "All Platforms", Key = "detail_platforms.md" },
        new FeatureItem { Name = "ViewModel-first navigation", Key = "detail_navigation.md" },
        new FeatureItem { Name = "Native view resolution", Key = "detail_navigation.md" },
        new FeatureItem { Name = "Navigation parameters", Key = "detail_navigation.md" },
        new FeatureItem { Name = "Back navigation", Key = "detail_navigation.md" },
        new FeatureItem { Name = "Navigation guards", Key = "detail_guards.md" },
        new FeatureItem { Name = "Dialog integration", Key = "detail_dialogs.md" },
        new FeatureItem { Name = "Generic factory support", Key = "detail_factory.md" },
        new FeatureItem { Name = "Dependency injection integration", Key = "detail_DI.md" },
        new FeatureItem { Name = "Logging support", Key = "detail_logging.md" }
    ];

    public FeatureService(IFileService fileService)
    {
        _fileService = fileService;
    }

    public Task<FeatureItem[]> GetFeaturesAsync()
    {
        return Task.FromResult<FeatureItem[]>(_features.ToArray());
    }

    public Task<FeatureItem> GetNextFeatureAsync(FeatureItem? item)
    {
        if(item == null)
        {
            return Task.FromResult(FeatureItem.Empty);
        }

        var index = _features.IndexOf(item);
        var nextIndex = (index + 1) % _features.Count;
        return Task.FromResult(_features[nextIndex]);
    }

    public Task<FeatureItem> GetPreviousFeatureAsync(FeatureItem? item)
    {
        if (item == null)
        {
            return Task.FromResult(FeatureItem.Empty);
        }
        var index = _features.IndexOf(item);
        var previousIndex = (index - 1 + _features.Count) % _features.Count;
        return Task.FromResult(_features[previousIndex]);
    }

    public async Task<FeatureItem> GetFeatureAsync(int id)
    {
        var result  = _features.FirstOrDefault(f => f.Id == id);
        var path = $".\\Markdown\\{result?.Key}";

        if (string.IsNullOrEmpty(result?.Markdown))
        {
            result?.Markdown = await _fileService.GetFileAsync(path);
        }
        return result ?? FeatureItem.Empty;
    }
}

public class FeatureItem
{
    private static int _idCounter = 0;
    public static FeatureItem Empty { get; } = new FeatureItem{Name = "Empty Feature"};
    public int Id { get; init; } = _idCounter++;
    public string Name { get; internal set; } = string.Empty;
    public string Key { get; internal set; } = string.Empty;
    public string Markdown { get; internal set; } = string.Empty;
}
