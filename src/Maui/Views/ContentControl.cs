using ADaxer.MvvmNav.Maui.Navigation;
using Microsoft.Maui.Controls;

namespace ADaxer.MvvmNav.Maui.Views;

/// <summary>
/// Displays a resolved view for its current binding context.
/// </summary>
/// <remarks>
/// When <see cref="IsDialog"/> is set to <see langword="true"/>,
/// the control resolves a dialog host view instead of a normal content view.
/// </remarks>
public class ContentControl : ContentView
{
    /// <summary>
    /// Identifies the <see cref="IsDialog"/> bindable property.
    /// </summary>
    public static readonly BindableProperty IsDialogProperty =
        BindableProperty.Create(
            nameof(IsDialog),
            typeof(bool),
            typeof(ContentControl),
            false,
            propertyChanged: OnIsDialogChanged);

    /// <summary>
    /// Gets or sets a value indicating whether the control should resolve
    /// a dialog host view instead of a normal content view.
    /// </summary>
    public bool IsDialog
    {
        get => (bool)GetValue(IsDialogProperty);
        set => SetValue(IsDialogProperty, value);
    }

    /// <inheritdoc />
    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        UpdateContent();
    }

    private static void OnIsDialogChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((ContentControl)bindable).UpdateContent();
    }

    private void UpdateContent()
    {
        if (BindingContext is null)
        {
            Content = null;
            return;
        }

        var view = IsDialog
            ? ViewLocator.Current.ResolveDialog(BindingContext) as View
            : ViewLocator.Current.ResolveView(BindingContext) as View;

        Content = view;
    }
}
