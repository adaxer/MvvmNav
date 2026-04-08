namespace ADaxer.MvvmNav.Sample.Common.Models;

public class FeatureItem
{
    private static int _idCounter = 0;
    public static FeatureItem Empty { get; } = new FeatureItem{Name = "Empty Feature"};
    public int Id { get; init; } = _idCounter++;
    public string Name { get; internal set; } = string.Empty;
    public string Key { get; internal set; } = string.Empty;
    public string Markdown { get; internal set; } = string.Empty;
}
