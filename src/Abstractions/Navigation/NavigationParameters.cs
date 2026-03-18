using System.Collections.ObjectModel;

namespace ADaxer.MvvmNav.Abstractions.Navigation;

public sealed class NavigationParameters
{
    public static NavigationParameters Empty { get; } = new();

    private IReadOnlyDictionary<string, object?> _parameters { get; }

    public NavigationParameters()
        : this(new Dictionary<string, object?>())
    {
    }

    public NavigationParameters(IDictionary<string, object?> parameters)
    {
        _parameters = new ReadOnlyDictionary<string, object?>(
            new Dictionary<string, object?>(parameters));
    }

    public NavigationParameters(params (string key, object? value)[] values)
        : this(values.ToDictionary(x => x.key, x => x.value))
    {
    }

    public object? this[string key]
        => _parameters.TryGetValue(key, out var value) ? value : null;

    public bool TryGetValue<T>(string key, out T? value)
    {
        if (_parameters.TryGetValue(key, out var raw) && raw is T typed)
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
}
