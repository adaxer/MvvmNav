using System.Windows;
using System.Windows.Controls;
using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Wpf.Views;

/// <summary>
/// Default WPF host window for dialogs shown by MvvmNav.
/// </summary>
/// <remarks>
/// The dialog relies on WPF data templates to resolve the actual content view
/// from the assigned dialog view model.
/// </remarks>
[TemplatePart(Name = "PART_YesButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_NoButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_CancelButton", Type = typeof(Button))]
public class WpfDialog : Window
{
    private bool? _dialogResult;

    /// <summary>
    /// Initializes static members of the <see cref="WpfDialog"/> class.
    /// </summary>
    static WpfDialog()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WpfDialog), new FrameworkPropertyMetadata(typeof(WpfDialog)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WpfDialog"/> class.
    /// </summary>
    public WpfDialog()
    {
        DataContextChanged += (_, _) =>
        {
            if (DataContext is IDialogCompletionSource completion)
            {
                CheckForCloseAsync(completion);
            }
        };
    }

    /// <summary>
    /// Polls the dialog completion task and closes the host window once it completed.
    /// </summary>
    /// <param name="completion">
    /// The dialog completion source.
    /// </param>
    private async void CheckForCloseAsync(IDialogCompletionSource completion)
    {
        do
        {
            await Task.Delay(300);
        }
        while (!completion.CompletionTask.IsCompleted);

        Close();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the standard command buttons are shown.
    /// </summary>
    public bool ShowCommands
    {
        get => (bool)GetValue(ShowCommandsProperty);
        set => SetValue(ShowCommandsProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="ShowCommands"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ShowCommandsProperty =
        DependencyProperty.Register(
            nameof(ShowCommands),
            typeof(bool),
            typeof(WpfDialog),
            new PropertyMetadata(false));

    /// <inheritdoc />
    public override void OnApplyTemplate()
    {
        if (GetTemplateChild("PART_YesButton") is Button yesButton)
        {
            yesButton.Click += (_, _) => DialogResult = _dialogResult = true;
        }

        if (GetTemplateChild("PART_NoButton") is Button noButton)
        {
            noButton.Click += (_, _) => DialogResult = _dialogResult = false;
        }

        if (GetTemplateChild("PART_CancelButton") is Button cancelButton)
        {
            cancelButton.Click += (_, _) => DialogResult = _dialogResult = null;
        }

        base.OnApplyTemplate();
    }

    /// <inheritdoc />
    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDialogCompletionSource completion && DataContext is IDialogController dialogController)
        {
            if (!completion.CompletionTask.IsCompleted)
            {
                dialogController.CloseDialog(new DialogResult(_dialogResult));
            }
        }

        base.OnClosed(e);
    }
}
