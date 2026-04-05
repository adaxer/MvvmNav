using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using Sample.Maui.Views;

namespace Sample.Maui;

public partial class App : Application
{
    internal IServiceProvider _serviceProvider = default!;

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(_serviceProvider.GetRequiredService<ShellPage>());
    }

    protected override async void OnStart()
    {
        base.OnStart();
        var navigation = _serviceProvider.GetRequiredService<INavigationService>();
        await navigation.NavigateAsync<HomeViewModel>();
    }


}
