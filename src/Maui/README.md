# ADaxer.MvvmNav.Maui

MAUI integration for **ADaxer.MvvmNav**, a lightweight MVVM navigation
framework for WPF, Avalonia and .NET MAUI.

This package provides the MAUI-specific implementation required to
connect the platform-independent navigation logic from `Core` with MAUI
views.

## Purpose

`ADaxer.MvvmNav.Maui` connects the navigation framework to MAUI by:

-   resolving Views for ViewModels
-   integrating with MAUI controls
-   enabling ViewModel-first navigation

## Basic Usage

``` csharp
services.AddMvvmNavMaui();
```

Views are resolved automatically based on the configured `IViewLocator`.

## Architecture

The framework is structured in layers:

-   `Abstractions` -- contracts
-   `Core` -- navigation logic
-   `Maui` -- MAUI-specific implementation
## Related Packages

-   `ADaxer.MvvmNav.Abstractions`
-   `ADaxer.MvvmNav.Core`
-   `ADaxer.MvvmNav.Avalonia`
-   `ADaxer.MvvmNav.Maui`

## License

Apache License 2.0
