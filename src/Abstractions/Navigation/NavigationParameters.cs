using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;

namespace ADaxer.MvvmNav.Abstractions.Navigation;

/// <summary>
/// Represents an immutable set of parameters passed to a navigation target.
/// </summary>
public sealed class NavigationParameters
{
    /// <summary>
    /// Gets an empty <see cref="NavigationParameters"/> instance.
    /// </summary>
    public static NavigationParameters Empty { get; } = new();

    private IReadOnlyDictionary<string, object?> _parameters { get; }

    /// <summary>
    /// Initializes a new empty instance of the <see cref="NavigationParameters"/> class.
    /// </summary>
    public NavigationParameters()
        : this(new Dictionary<string, object?>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationParameters"/> class
    /// from the specified parameter dictionary.
    /// </summary>
    /// <param name="parameters">The parameters to copy.</param>
    public NavigationParameters(IDictionary<string, object?> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        _parameters = new ReadOnlyDictionary<string, object?>(
            new Dictionary<string, object?>(parameters));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationParameters"/> class
    /// from the specified key/value pairs.
    /// </summary>
    /// <param name="values">The parameter values to copy.</param>
    public NavigationParameters(params (string key, object? value)[] values)
        : this(values.ToDictionary(x => x.key, x => x.value))
    {
    }

    /// <summary>
    /// Gets the parameter value associated with the specified key,
    /// or <c>null</c> if the key does not exist.
    /// </summary>
    /// <param name="key">The parameter key.</param>
    public object? this[string key]
        => _parameters.TryGetValue(key, out var value) ? value : null;

    /// <summary>
    /// Tries to get the parameter value associated with the specified key
    /// and cast it to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The expected parameter type.</typeparam>
    /// <param name="key">The parameter key.</param>
    /// <param name="value">
    /// When this method returns, contains the typed value if the key exists
    /// and the value can be cast successfully; otherwise, the default value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the key exists and the value is of type <typeparamref name="T"/>;
    /// otherwise, <c>false</c>.
    /// </returns>
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

    /// <summary>
    /// Gets the parameter value associated with the specified key
    /// and casts it to <typeparamref name="T"/>, or returns the default value.
    /// </summary>
    /// <typeparam name="T">The expected parameter type.</typeparam>
    /// <param name="key">The parameter key.</param>
    /// <returns>
    /// The typed value if the key exists and the value can be cast successfully;
    /// otherwise, the default value.
    /// </returns>
    public T? GetValueOrDefault<T>(string key)
    {
        return TryGetValue<T>(key, out var value) ? value : default;
    }

    /// <summary>
    /// Returns a normalized string representation of all parameters.
    /// Keys are sorted ordinally, values are converted using invariant formatting
    /// when available, and reserved separator characters are escaped.
    /// </summary>
    /// <returns>A stable string representation of the contained parameters.</returns>
    public string ToNormalizedString()
    {
        if (_parameters.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();

        foreach (var pair in _parameters.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            if (builder.Length > 0)
            {
                builder.Append('|');
            }

            builder
                .Append(Escape(pair.Key))
                .Append('=')
                .Append(Escape(NormalizeValue(pair.Value)));
        }

        return builder.ToString();
    }

    private static string NormalizeValue(object? value)
    {
        return value switch
        {
            null => "null",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? "null",
            _ => value.ToString() ?? "null"
        };
    }

    private static string Escape(string value)
    {
        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("|", "\\|", StringComparison.Ordinal)
            .Replace("=", "\\=", StringComparison.Ordinal);
    }
}
