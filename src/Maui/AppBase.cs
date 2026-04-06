using ADaxer.MvvmNav.Abstractions.Navigation;

namespace ADaxer.MvvmNav.Maui;

public abstract class AppBase : Application
{
    private IServiceProvider _serviceProvider = default!;
    private INavigationService _navigationService;

    public AppBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected IServiceProvider ServiceProvider { get => _serviceProvider; }
    protected INavigationService NavigationService { get => _navigationService; }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Application.Current.Resources.MergedDictionaries.Add(new MauiResources());
        return new Window(ServiceProvider.GetRequiredService<IShellView>() as Page);
    }

    protected override async void OnStart()
    {
        base.OnStart();
        _navigationService = ServiceProvider.GetRequiredService<INavigationService>();
        await PostCreateAsync();
    }

    protected abstract Task PostCreateAsync();
}

