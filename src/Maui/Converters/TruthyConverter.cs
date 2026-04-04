using System.Globalization;

namespace ADaxer.MvvmNav.Maui.Converters;

/// <summary>
/// Converts various values to a boolean based on "truthy" semantics.
/// </summary>
/// <remarks>
/// Returns true for:
/// <list type="bullet">
/// <item><description><see cref="bool"/> true</description></item>
/// <item><description>non-null objects</description></item>
/// <item><description>non-empty strings</description></item>
/// <item><description>numeric values not equal to zero</description></item>
/// </list>
/// Otherwise returns false.
/// </remarks>
public class TruthyConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
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

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b;

        return false;
    }
}
