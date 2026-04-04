using ADaxer.MvvmNav.Abstractions.Navigation;

namespace Sample.Maui.Views;

public partial class ShellPage : ContentPage, IShellView
{
    public ShellPage(IShellViewModel viewModel)
    {
        InitializeComponent();
        DataContext  = viewModel;
    }

    public object? DataContext
    {
        get => BindingContext;
        set => BindingContext = value;
    }

    public void Show()
    {
        // In MAUI nicht aktiv nötig.
        // Die App setzt diese Page im Window.
    }
}
