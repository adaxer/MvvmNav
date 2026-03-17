# ADaxer.MvvmNav -- Context Summary

## Project Goal

**ADaxer.MvvmNav** is a lightweight MVVM navigation framework for .NET
UI applications.

It is a nuget package, and the project site is OSS on 
https://github.com/adaxer/MvvmNav
 
It focuses on simplicity, clear responsibilities, and minimal
infrastructure while integrating naturally with the UI platform.

The framework is designed to work across multiple UI technologies.

### Supported Platforms (Target)

-   **Avalonia** (Desktop and Mobile)
-   **WPF**
-   **MAUI**
-   **Uno Platform** (Desktop and Mobile)

The core navigation logic is platform‑agnostic, while the UI layer uses
the native mechanisms of the respective platform.

------------------------------------------------------------------------

# Architecture Overview

## Core Components

### NavigationService

Central orchestrator responsible for:

-   navigation between ViewModels
-   maintaining a navigation back stack
-   evaluating navigation guards (`ICanNavigateFrom`)
-   integrating dialog results
-   creating ViewModels via dependency injection

The NavigationService intentionally acts as the **single orchestration
point**.

------------------------------------------------------------------------

### DialogService

Responsible for:

-   showing modal dialogs
-   hosting dialog views
-   returning a `DialogResult`

Dialog views are resolved using the platform's native templating system.

------------------------------------------------------------------------

# View Resolution Strategy

The framework **does not implement a custom ViewResolver**.

Instead it relies on the native UI platform mechanism:

    ViewModel
       ↓
    ContentControl
       ↓
    DataTemplate (WPF/Avalonia/Uno/etc.)
       ↓
    View

Example:

    <DataTemplate DataType="{x:Type vm:SettingsViewModel}">
        <views:SettingsView/>
    </DataTemplate>

This keeps the framework simple and predictable for developers familiar
with the platform.

------------------------------------------------------------------------

# Navigation Guards

ViewModels can prevent navigation via:

    ICanNavigateFrom

Return type:

    NavigationGuardResult

Possible decisions:

-   `Allow`
-   `Disallow`
-   `AskUser`

------------------------------------------------------------------------

## AskUser Flow

When a ViewModel returns `AskUser`:

1.  `NavigationService` shows a dialog
2.  The dialog returns a `DialogResult`
3.  The result is passed back to the ViewModel through a continuation
    callback

Callback signature:

    Func<DialogResult, CancellationToken, Task>

This allows the ViewModel to continue the decision asynchronously
(e.g. saving changes).

------------------------------------------------------------------------

# Dialog Model

Dialog ViewModels implement:

    IDialogAware

Public API:

    CloseDialog(bool result)

Technical dialog completion is handled internally by:

    IDialogCompletionSource

which encapsulates the `TaskCompletionSource<bool>` mechanism.

------------------------------------------------------------------------

# Base Classes

Base classes are **optional convenience helpers**.

The framework itself works primarily with interfaces.

## ViewModelBase

Derived from:

    ObservableObject (CommunityToolkit.Mvvm)

Provides commonly useful properties:

-   `Title`
-   `IsBusy`

------------------------------------------------------------------------

## DialogViewModelBase

Derived from:

    ViewModelBase

Implements:

-   `IDialogAware`
-   `IDialogCompletionSource`

This base class hides the dialog completion infrastructure.

------------------------------------------------------------------------

# Navigation Lifecycle

ViewModels may implement:

    INavigationAware

Lifecycle method:

    Task OnNavigatedToAsync(NavigationParameters parameters)

The same hook is used for both normal navigation and dialog navigation.

------------------------------------------------------------------------

# Platform Integration

View resolution relies on native templating systems.

Example for WPF/Avalonia:

    ContentControl Content="{Binding CurrentModule}"

The DataTemplate associated with the ViewModel type resolves the View
automatically.

------------------------------------------------------------------------

# Dialog Hosting Example

Dialogs are hosted by assigning the ViewModel to the dialog DataContext
and binding the dialog Content to the ViewModel itself.

Example concept:

    dlg.DataContext = dialogViewModel
    dlg.SetBinding(ContentProperty, ".")

The `"."` binding path binds the ViewModel itself, enabling DataTemplate
resolution.

------------------------------------------------------------------------

# Messaging / PubSub

Loose communication between components is implemented using:

    CommunityToolkit.Mvvm IMessenger

Typical use case:

-   status messages
-   navigation notifications
-   dialog results

Example: updating a shell status bar.

------------------------------------------------------------------------

# Future Extensions (not part of v1)

Possible future additions:

-   `CanNavigateTo`
-   navigation redirect
-   nested navigation regions
-   wizard flows
-   authorization guards

These are intentionally left out of version 1 to keep the framework
simple.

------------------------------------------------------------------------

# Design Principles

Key architectural principles:

1.  **Interface‑first design**\
    Base classes are optional.

2.  **Use platform mechanisms**\
    Native DataTemplates are used for View resolution.

3.  **Minimal infrastructure**\
    Only a few core services exist:

    -   `NavigationService`
    -   `DialogService`

4.  **ViewModel‑first navigation**\
    Navigation always targets ViewModels.

5.  **Small and understandable framework**\
    The goal is not to replicate large frameworks like Prism.

------------------------------------------------------------------------

# Sample Application Goals

The sample application demonstrates all major features in a simple way.

## Modules

### Shell

Shows:

-   navigation
-   back stack
-   status bar

------------------------------------------------------------------------

### Home

Landing page introducing navigation.

------------------------------------------------------------------------

### Details

Demonstrates:

-   `NavigationParameters`
-   `INavigationAware`

------------------------------------------------------------------------

### Settings

Demonstrates:

-   dirty state tracking
-   `ICanNavigateFrom`
-   `AskUser` confirmation dialog
-   save / discard / cancel scenarios

------------------------------------------------------------------------

### About

Demonstrates:

-   simple dialog
-   `DialogViewModelBase`

------------------------------------------------------------------------

### PlainViewModel Example

Demonstrates usage **without framework base classes**, using only
interfaces.

------------------------------------------------------------------------

# Optional Sample Features

### Status Bar

Implemented using:

    IMessenger

Displays:

-   navigation events
-   dialog results
-   guard cancellations

------------------------------------------------------------------------

### Factory Pattern

Optional example:

    IFactory<T>

Example use case:

    EditCustomerViewModel

------------------------------------------------------------------------

# Key Takeaways

Core ideas of the framework:

-   Navigation is orchestrated by `NavigationService`
-   Views are resolved by platform DataTemplates
-   Dialogs are hosted by `DialogService`
-   Navigation guards control navigation flow
-   Base classes are optional helpers
-   CommunityToolkit.Mvvm provides MVVM infrastructure

------------------------------------------------------------------------

# Result

The framework aims to be:

-   small
-   understandable
-   platform‑agnostic
-   extensible
-   easy to integrate

This constitutes a solid **version 1 architecture**.
