using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Maui;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using Sample.Maui.Views;

namespace Sample.Maui;

public partial class App : AppBase
{
    public App(IServiceProvider serviceProvider)
        : base(serviceProvider) 
    {
        InitializeComponent();
    }

    protected override async Task PostCreateAsync()
    {
        await NavigationService.NavigateAsync<HomeViewModel>();
    }
}

