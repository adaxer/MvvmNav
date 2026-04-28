# ADaxer.MvvmNav.Avalonia

Avalonia integration for **ADaxer.MvvmNav**, a lightweight MVVM navigation
framework for WPF, Avalonia and .NET MAUI.

This package provides the Avalonia-specific implementation required to
connect the platform-independent navigation logic from `Core` with Avalonia
views.

## Purpose

`ADaxer.MvvmNav.Avalonia` connects the navigation framework to Avalonia by:

-   resolving Views for ViewModels
-   integrating with Avalonia controls
-   enabling ViewModel-first navigation

## Basic Usage

``` csharp
services.AddMvvmNavAvalonia();
```

## Architecture

The framework is structured in layers:

-   `Abstractions` -- contracts
-   `Core` -- navigation logic
-   `Avalonia` -- Avalonia-specific implementation

## Related Packages

-   `ADaxer.MvvmNav.Abstractions`
-   `ADaxer.MvvmNav.Core`
-   `ADaxer.MvvmNav.Wpf`
-   `ADaxer.MvvmNav.Maui`

## License

Apache License 2.0
