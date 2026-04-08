using ADaxer.MvvmNav.Abstractions.Navigation;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Avalonia;

public abstract class AvaloniaAppBase : Application
{
    private IServiceProvider _serviceProvider = default!;
    private INavigationService _navigationService;

    public AvaloniaAppBase(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected IServiceProvider ServiceProvider { get => _serviceProvider; }
    protected INavigationService NavigationService { get => _navigationService; }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            //desktop.MainWindow = new MainWindow
            //{
            //    DataContext = new MainViewModel()
            //};
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            //singleViewPlatform.MainView = new MainView
            //{
            //    DataContext = new MainViewModel()
            //};
        }
        base.OnFrameworkInitializationCompleted();
    }
    //protected override Window CreateWindow(IActivationState? activationState)
    //{
    //    Application.Current.Resources.MergedDictionaries.Add(new MauiResources());
    //    return new Window(ServiceProvider.GetRequiredService<IShellView>() as Page);
    //}

    //protected override async void OnStart()
    //{
    //    base.OnStart();
    //    _navigationService = ServiceProvider.GetRequiredService<INavigationService>();
    //    await PostCreateAsync();
    //}

    protected abstract Task PostCreateAsync();
}
