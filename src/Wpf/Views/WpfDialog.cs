using System.Windows;
using System.Windows.Controls;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;

namespace ADaxer.MvvmNav.Wpf.Views;

[TemplatePart(Name = "PART_YesButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_NoButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_CancelButton", Type = typeof(Button))]
public class WpfDialog : Window
{
    private bool? _dialogResult = null;

    static WpfDialog()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WpfDialog), new FrameworkPropertyMetadata(typeof(WpfDialog)));
    }

    public WpfDialog()
    {
        DataContextChanged += (_, _) =>
        {
            if (DataContext is IDialogCompletionSource completion) CheckForCloseAsync(completion);
        };
        _dialogResult = null;
    }

    private async void CheckForCloseAsync(IDialogCompletionSource completion)
    {
        do
        {
            await Task.Delay(300);
        } while (!completion.CompletionTask.IsCompleted);

        Close();
    }

    public bool ShowCommands
    {
        get { return (bool)GetValue(ShowCommandsProperty); }
        set { SetValue(ShowCommandsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ShowCommands.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShowCommandsProperty =
        DependencyProperty.Register("ShowCommands", typeof(bool), typeof(WpfDialog), new PropertyMetadata(false));

    public override void OnApplyTemplate()
    {
        if (GetTemplateChild("PART_YesButton") is Button yesButton)
        {
            yesButton.Click += (s, e) => DialogResult = _dialogResult = true;
        }
        if (GetTemplateChild("PART_NoButton") is Button noButton)
        {
            noButton.Click += (s, e) => DialogResult = _dialogResult = false;
        }
        if (GetTemplateChild("PART_CancelButton") is Button cancelButton)
        {
            cancelButton.Click += (s, e) => DialogResult = _dialogResult = null;
        }
        base.OnApplyTemplate();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDialogCompletionSource completion && DataContext is IDialogAware dialogAware)
        {
            if (!completion.CompletionTask.IsCompleted)
            {
                    dialogAware.CloseDialog(new Abstractions.Navigation.DialogResult(_dialogResult));
            }
        }
        base.OnClosed(e);
    }
}
