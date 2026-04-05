using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ADaxer.MvvmNav.Wpf.Converters;

/// <summary>
/// Converts various values to a <see cref="Visibility"/> based on "truthy" semantics.
/// </summary>
/// <remarks>
/// Returns <see cref="Visibility.Visible"/> for:
/// <list type="bullet">
/// <item><description><see cref="bool"/> true</description></item>
/// <item><description>non-null objects</description></item>
/// <item><description>non-empty strings</description></item>
/// <item><description>numeric values not equal to zero</description></item>
/// </list>
/// Otherwise returns either <see cref="Visibility.Collapsed"/> or
/// <see cref="Visibility.Hidden"/> depending on <see cref="FalseValue"/>.
/// </remarks>
public class TruthyToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets how false values are represented.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="FalseVisibilityMode.Collapsed"/>.
    /// </remarks>
    public FalseVisibilityMode FalseValue { get; set; } = FalseVisibilityMode.Collapsed;

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isTruthy = IsTruthy(value);

        if (isTruthy)
            return Visibility.Visible;

        return FalseValue == FalseVisibilityMode.Hidden
            ? Visibility.Hidden
            : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Visibility v)
            return v == Visibility.Visible;

        return false;
    }

    private static bool IsTruthy(object? value)
    {
        if (value is null)
            return false;

        if (value is bool b)
            return b;

        if (value is string s)
            return !string.IsNullOrWhiteSpace(s);

        if (value is int i)
            return i != 0;

        if (value is long l)
            return l != 0;

        if (value is double d)
            return Math.Abs(d) > double.Epsilon;

        if (value is float f)
            return Math.Abs(f) > float.Epsilon;

        if (value is decimal m)
            return m != 0;

        return true; // alles andere: non-null => true
    }
}

/// <summary>
/// Specifies how a converter should represent a false value as <see cref="Visibility"/>.
/// </summary>
public enum FalseVisibilityMode
{
    /// <summary>
    /// Maps false to <see cref="Visibility.Collapsed"/>.
    /// </summary>
    Collapsed = 0,

    /// <summary>
    /// Maps false to <see cref="Visibility.Hidden"/>.
    /// </summary>
    Hidden = 1
}
