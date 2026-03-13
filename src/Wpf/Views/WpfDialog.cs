using System.Windows;
using System.Windows.Controls;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core.ViewModels;

namespace ADaxer.MvvmNav.Wpf.Views;

public class WpfDialog : Window
{
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
        if (GetTemplateChild("PART_OkButton") is Button okButton)
        {
            okButton.Click += (s, e) => DialogResult = true;
        }
        base.OnApplyTemplate();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDialogCompletionSource completion && DataContext is IDialogAware dialogAware)
        {
            if (!completion.CompletionTask.IsCompleted)
            {
                dialogAware.CloseDialog(false);
            }
        }
        base.OnClosed(e);
    }
}
