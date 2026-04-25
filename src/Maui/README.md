# ADaxer.MvvmNav.Wpf

WPF integration for **ADaxer.MvvmNav**, a lightweight MVVM navigation
framework for WPF, Avalonia and .NET MAUI.

This package provides the WPF-specific implementation required to
connect the platform-independent navigation logic from `Core` with WPF
views.

## Purpose

`ADaxer.MvvmNav.Wpf` connects the navigation framework to WPF by:

-   resolving Views for ViewModels
-   integrating with WPF controls
-   enabling ViewModel-first navigation

## Basic Usage

``` csharp
services.AddMvvmNavWpf();
```

Views are resolved automatically based on the configured `IViewLocator`.

## Architecture

The framework is structured in layers:

-   `Abstractions` -- contracts
-   `Core` -- navigation logic
-   `Wpf` -- WPF-specific implementation

## Related Packages

-   `ADaxer.MvvmNav.Abstractions`
-   `ADaxer.MvvmNav.Core`
-   `ADaxer.MvvmNav.Avalonia`
-   `ADaxer.MvvmNav.Maui`

## License

Apache License 2.0
