# ADaxer.MvvmNav.Core

Core implementation of **ADaxer.MvvmNav**, a lightweight MVVM navigation
framework for WPF, Avalonia and .NET MAUI.

This package contains the platform-independent navigation logic built on
top of the abstractions.

## Purpose

`ADaxer.MvvmNav.Core` provides a clean, ViewModel-first navigation model
with minimal boilerplate.

It is designed to:

-   separate navigation logic from UI frameworks
-   keep ViewModels platform-independent
-   provide a consistent navigation experience across platforms

## Basic Usage

``` csharp
public class MainViewModel
{
    private readonly INavigationService _navigation;

    public MainViewModel(INavigationService navigation)
    {
        _navigation = navigation;
    }

    public Task OpenDetailsAsync()
    {
        return _navigation.NavigateAsync("Details");
    }
}
```

## Architecture

The framework is structured in layers:

-   `Abstractions` -- contracts (interfaces, models)
-   `Core` -- navigation logic and orchestration
-   Platform packages -- UI integration (WPF, Avalonia, MAUI)

## Related Packages

-   `ADaxer.MvvmNav.Abstractions` -- required contracts
-   `ADaxer.MvvmNav.Wpf`
-   `ADaxer.MvvmNav.Avalonia`
-   `ADaxer.MvvmNav.Maui`

## License

Apache License 2.0
