// UpVersion.cs
#:package Cocona

using Cocona;
using System.Text.RegularExpressions;
using System.Xml.Linq;

CoconaApp.Run((
    string? version = null,
    [Option('p')] string[]? path = null) =>
{
    var mode = ParseVersionMode(version);
    var pathFilters = path ?? Array.Empty<string>();

    var files = Directory
        .GetFiles(Environment.CurrentDirectory, "*.csproj", SearchOption.AllDirectories)
        .Where(file => MatchesAnyPathFilter(file, pathFilters))
        .ToList();

    foreach (var file in files)
    {
        UpdateProjectVersion(file, mode);
    }
});

static VersionMode ParseVersionMode(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
        return VersionMode.IncrementBuild();

    return value switch
    {
        "1" => VersionMode.IncrementMajor(),
        "2" => VersionMode.IncrementMinor(),
        "3" => VersionMode.IncrementBuild(),
        _ when Version.TryParse(value, out var v) => VersionMode.SetExplicit(v),
        _ => throw new ArgumentException($"Invalid version value: {value}")
    };
}

static bool MatchesAnyPathFilter(string file, string[] filters)
{
    if (filters.Length == 0)
        return true;

    var normalized = file.Replace('\\', '/');

    return filters.Any(filter =>
    {
        var pattern = filter.Contains('*')
            ? filter
            : $"*{filter}*";

        return WildcardMatch(normalized, pattern)
            || WildcardMatch(Path.GetFileName(file), pattern);
    });
}

static bool WildcardMatch(string input, string pattern)
{
    var regex = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
    return Regex.IsMatch(input, regex, RegexOptions.IgnoreCase);
}

static void UpdateProjectVersion(string file, VersionMode mode)
{
    var doc = XDocument.Load(file);
    var ns = doc.Root?.Name.Namespace ?? XNamespace.None;

    var versionElement = doc.Descendants(ns + "Version").FirstOrDefault();

    if (versionElement == null)
    {
        Console.WriteLine($"No <Version> in {file}");
        return;
    }

    if (!Version.TryParse(versionElement.Value, out var current))
    {
        Console.WriteLine($"Invalid version in {file}: {versionElement.Value}");
        return;
    }

    var newVersion = ApplyVersionMode(current, mode);

    versionElement.Value = newVersion.ToString();
    doc.Save(file);

    Console.WriteLine($"{file}: {current} -> {newVersion}");
}

static Version ApplyVersionMode(Version current, VersionMode mode)
{
    if (mode.ExplicitVersion is not null)
        return mode.ExplicitVersion;

    return mode.Kind switch
    {
        VersionModeKind.IncrementMajor => new Version(current.Major + 1, 0, 0),
        VersionModeKind.IncrementMinor => new Version(current.Major, current.Minor + 1, 0),
        _ => new Version(current.Major, current.Minor, current.Build + 1)
    };
}

record VersionMode(VersionModeKind Kind, Version? ExplicitVersion)
{
    public static VersionMode IncrementMajor() => new(VersionModeKind.IncrementMajor, null);
    public static VersionMode IncrementMinor() => new(VersionModeKind.IncrementMinor, null);
    public static VersionMode IncrementBuild() => new(VersionModeKind.IncrementBuild, null);
    public static VersionMode SetExplicit(Version v) => new(VersionModeKind.Explicit, v);
}

enum VersionModeKind
{
    IncrementMajor,
    IncrementMinor,
    IncrementBuild,
    Explicit
}
