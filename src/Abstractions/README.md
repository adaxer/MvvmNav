# ADaxer.MvvmNav.Abstractions

Abstractions for **ADaxer.MvvmNav**, a lightweight MVVM navigation
framework for WPF, Avalonia and .NET MAUI.

This package defines the core contracts used by the framework, including
navigation, dialog handling and view resolution.

## Purpose

The abstractions provide a clean, platform-independent foundation for
MVVM navigation.

They are intended to be implemented by platform-specific packages such
as:

-   `ADaxer.MvvmNav.Wpf`
-   `ADaxer.MvvmNav.Avalonia`
-   `ADaxer.MvvmNav.Maui`

## Example

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

## Related Packages

-   `ADaxer.MvvmNav.Core` -- core navigation logic
-   Platform packages for WPF, Avalonia and .NET MAUI

## License

Apache License 2.0
