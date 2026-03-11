using System.Collections.ObjectModel;

namespace ADaxer.MvvmNav.Abstractions.Navigation;

public sealed class NavigationContext
{
    public static NavigationContext Empty { get; } = new();

    public IReadOnlyDictionary<string, object?> Parameters { get; }

    public NavigationContext()
        : this(new Dictionary<string, object?>())
    {
    }

    public NavigationContext(IDictionary<string, object?> parameters)
    {
        Parameters = new ReadOnlyDictionary<string, object?>(
            new Dictionary<string, object?>(parameters));
    }

    public object? this[string key]
        => Parameters.TryGetValue(key, out var value) ? value : null;

    public bool TryGetValue<T>(string key, out T? value)
    {
        if (Parameters.TryGetValue(key, out var raw) && raw is T typed)
        {
            value = typed;
            return true;
        }

        value = default;
        return false;
    }

    public T? GetValueOrDefault<T>(string key)
    {
        return TryGetValue<T>(key, out var value) ? value : default;
    }

    public static NavigationContext From(params (string key, object? value)[] values)
    {
        var dictionary = values.ToDictionary(x => x.key, x => x.value);
        return new NavigationContext(dictionary);
    }
}
