# ADaxer.MvvmNav -- Context Summary

This document reflects the current architectural direction and recent design decisions.

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

## Recent Decisions / Current State

### Logging
- The framework supports `Microsoft.Extensions.Logging`.
- Core services (e.g. `NavigationService`) may use `ILogger<T>`.
- Concrete logging providers are configured at application/bootstrap level.
- The framework itself does not enforce a specific logging provider.

### Registration Order
- Recommended registration order:
  1. Core
  2. Platform
  3. Application
- Later registrations override earlier ones.
- This allows application-level customization without modifying the framework.

### WPF Host Builder
- WPF provides an optional fluent host builder:
  `WpfNavigationHostBuilder`
- Intended usage:
  - `WpfNavigationHostBuilder.BuildDefault<TShellView, TShellViewModel>().Start()`
  - `WpfNavigationHostBuilder.Build<TShellView, TShellViewModel>().WithLogging(...).WithServices(...).Start()`
- The host builder is optional and serves as a convenience layer.
- Existing applications can integrate MvvmNav without using it.

### Navigation State Notifications
- `INavigationService` exposes navigation state change notifications.
- These are triggered after:
  - successful navigation
  - successful back navigation
- Not triggered for dialogs.
- Typical use case:
  - updating shell commands (e.g. Back button)
  - refreshing `CanExecute` state

### Navigation Parameter Convenience
- Navigation can be invoked using tuple-based parameters:
  `NavigateAsync<TTarget>(("Key", value), ("Other", 42))`
- This avoids explicit construction of `NavigationParameters` in common scenarios.
- Implemented via extension methods on `INavigationService`.

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

## Navigation Semantics

- Navigation is ViewModel-to-ViewModel via `NavigateAsync(...)`.
- Back navigation uses `GoBackAsync()`.
- Dialogs are separate via `ShowDialogAsync(...)` (no modal flag on navigation).

### Target Identity

A navigation target is identified by:
- `TargetType`
- `NavigationKey`

Navigation to the same target is blocked only if **both match**.

Examples:
- Same type, different key (e.g. different Id) → allowed
- Same type, same key → blocked

### NavigationKey

- Default: `TargetType + normalized NavigationParameters`
- Custom: `NavigationOptions.WithKey("...")`

Use a custom key when:
- only a subset of parameters defines identity
- parameters contain complex objects
- `ToString()` is not stable enough

### NavigationParameters

- Immutable parameter bag
- Used to pass context and (by default) define identity
- Prefer primitive/value-like data (Id, Filter, Page, Mode)
- For complex cases, provide a custom `NavigationKey`

### Back Stack

Each entry stores:
- Target instance
- TargetType
- Parameters
- NavigationKey

`GoBackAsync()` restores the original semantic target (type + parameters + key).

### Detail Paging Scenario

- Same ViewModel type is allowed if the target identity changes
- Example: `Detail(Id=10)` → `Detail(Id=11)` allowed
- `Detail(Id=10)` → `Detail(Id=10)` blocked


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

# Roadmap

## Featureset v1

- **ViewModel-first navigation**  
  Navigate between ViewModels without coupling them to concrete views.

- **Platform-agnostic core**  
  The navigation engine resides in a UI-independent Core library.

- **Native view resolution**  
  Views are resolved using the platform’s native mechanisms (e.g. WPF `DataTemplate`), not a custom view locator.

- **Navigation parameters**  
  Pass parameters when navigating between ViewModels.

- **Back navigation with stack management**  
  Built-in back stack with support for clearing or suppressing entries.

- **Navigation guards**  
  ViewModels can intercept navigation using `ICanNavigateFrom` to allow, deny, or request user confirmation.

- **Dialog integration**  
  Unified dialog workflow via `IDialogService` with typed results.

- **Async navigation lifecycle**  
  `INavigationAware` enables asynchronous initialization after navigation.

- **Generic factory support (`IFactory<T>`)**  
  Provides a reusable DI-backed factory abstraction for creating ViewModels or other services.

- **Microsoft.Extensions.DependencyInjection integration**  
  Designed to work naturally with the standard .NET dependency injection container.

- **Microsoft.Extensions.Logging support**  
  Built-in logging using the standard .NET logging abstractions.

- **Navigation state notifications**  
  The navigation service exposes events when the navigation state changes, enabling UI elements (e.g. back buttons) to update automatically.

- **Optional WPF host builder**  
  `WpfNavigationHostBuilder` enables quick setup with fluent configuration.

- **Framework-agnostic usage**  
  Can be integrated into existing bootstrapping processes without using the host builder.


## Planned

- **CanNavigateTo with Authorization and Redirect**  
  Allows ViewModels to control whether navigation to a target is permitted and optionally redirect to another ViewModel (e.g. login flow).

- **Initialization Lifecycle (early/late, sync/async)**  
  Supports structured ViewModel initialization stages before and after navigation with both synchronous and asynchronous execution.

- **SaveState / RestoreState**  
  Enables persisting and restoring ViewModel or navigation state, useful for application restart or suspend/resume scenarios.

- **IPubSub Service**  
  Lightweight publish/subscribe messaging service for decoupled communication between ViewModels.

- **Navigation Events**  
  Exposes events such as `Navigating`, `Navigated`, and `NavigationFailed` for diagnostics, telemetry, and external orchestration.

- **Typed Navigation Parameters**  
  Supports strongly typed navigation parameters to avoid string-based parameter keys.

---

### Under Consideration

- **Navigation Scopes**  
  Allows creating scoped navigation contexts (e.g. wizard flows or multi-step processes) with isolated DI scopes.

- **Navigation History API**  
  Exposes navigation history and back stack information for debugging, breadcrumbs, or custom navigation UI.

- **Navigation Cancellation Support**  
  Adds `CancellationToken` support to navigation operations to cancel long-running initialization tasks.

- **Diagnostics / Debug View**  
  Provides a developer-oriented diagnostics API to inspect current navigation state, back stack, and active ViewModels.

- **Optional Route-Based Navigation**  
  Allows mapping routes (e.g. `/orders/42`) to ViewModels for scenarios where route-style navigation is desirable.

- **ViewModel Activation Policies**  
  Supports controlling ViewModel lifetime (e.g. reuse existing instances, single-instance ViewModels).
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
## Sample App – Current Direction

### General Approach
- Prefer a single conceptual sample application shared across platforms.
- Platform-specific integration is demonstrated per platform:
  - WPF: `WpfNavigationHostBuilder` + Serilog
  - Avalonia: default builder
  - MAUI: integration into existing app bootstrap
  - Uno: planned after WPF/Avalonia/MAUI

### Shell Navigation
- The shell provides a left navigation menu.
- Each menu item consists of:
  - Title
  - Subtitle
  - Command
- Current modules:
  - Home
  - About
  - Settings
  - Features

### Markdown-based Explanations
- Sample pages use markdown for inline documentation.
- Markdown is bound via a ViewModel property (`string Markdown`).
- Each platform uses a platform-specific markdown renderer.
- Feature detail pages load markdown files based on a navigation parameter key.

### Sample Content Focus
- Home:
  - MvvmNav SampleApp overview
  - ViewModel-first navigation
  - ShellView / ShellViewModel only require framework interfaces
  - NavigationService is injected into the ShellViewModel
  - Views are resolved via DataTemplates
- About:
  - Demonstrates dialog usage
- Settings:
  - Demonstrates navigation and back navigation
- Features:
  - Overview page with links to detail pages
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

------------------------------------------------------------------------

# Testing

## Next Steps for `NavigationService`-Tests

### `NavigationService_BackNavigation__`

- `CanGoBack_WithoutPreviousNavigation_ShouldReturnFalse`
- `CanGoBack_AfterSecondNavigation_ShouldReturnTrue`
- `GoBackAsync_AfterNavigatingToSecondTarget_ShouldRestorePreviousTarget`
- `GoBackAsync_ShouldRestorePreviousParameters`
- `GoBackAsync_WhenBackStackBecomesEmpty_ShouldUpdateCanGoBack`
- `GoBackAsync_WhenCurrentTargetDisallowsNavigation_ShouldNotGoBack`
- `GoBackAsync_WhenCurrentTargetRequestsConfirmation_AndUserDeclines_ShouldNotGoBack`
- `GoBackAsync_WhenCurrentTargetRequestsConfirmation_AndUserConfirms_ShouldGoBack`
- `GoBackAsync_ShouldPassIsBackNavigationTrueToNavigationGuard`

### `NavigationService_Guards__`

- `NavigateAsync_WhenCurrentTargetAllowsNavigation_ShouldNavigate`
- `NavigateAsync_WhenCurrentTargetDisallowsNavigation_ShouldNotNavigate`
- `NavigateAsync_WhenCurrentTargetRequestsConfirmation_AndUserDeclines_ShouldNotNavigate`
- `NavigateAsync_WhenCurrentTargetRequestsConfirmation_AndUserConfirms_ShouldNavigate`
- `NavigateAsync_WhenGuardRequestsConfirmationWithoutContext_ShouldThrow`
- `NavigateAsync_WhenNavigationIsBlocked_ShouldNotRaiseNavigationStateChanged`
- `NavigateAsync_WhenNavigationIsBlocked_ShouldNotPushCurrentTargetToBackStack`

### `NavigationService_BackStackOptions__`

- `NavigateAsync_WithAddToBackStackFalse_ShouldNotAddCurrentTargetToBackStack`
- `NavigateAsync_WithAddToBackStackFalse_ShouldMakeGoBackUnavailable`
- `NavigateAsync_WithClearBackStackTrue_ShouldClearExistingBackStack`
- `NavigateAsync_WithClearBackStackTrue_AndThenNavigate_ShouldLeaveOnlyExpectedBackState`
- `NavigateAsync_WithExplicitNavigationKey_ShouldUseItForBackStackIdentity`

### `NavigationService_Events__`

- `NavigateAsync_AfterSuccessfulNavigation_ShouldRaiseNavigationStateChanged`
- `GoBackAsync_AfterSuccessfulBackNavigation_ShouldRaiseNavigationStateChanged`
- `NavigateAsync_WhenNavigatingToSameTargetAndSameKey_ShouldNotRaiseNavigationStateChanged`
- `ShowDialogAsync_ShouldNotRaiseNavigationStateChanged`

### `NavigationService_Activation__`

- `NavigateAsync_ShouldSetShellCurrentModuleToResolvedTarget`
- `NavigateAsync_WithNavigationAwareTarget_ShouldPassParametersToOnNavigatedToAsync`
- `NavigateAsync_WithTargetNotImplementingNavigationAware_ShouldStillActivate`
- `GoBackAsync_WithNavigationAwareTarget_ShouldPassRestoredParametersToOnNavigatedToAsync`

### `DialogViewModelBase__`

- `CompletionTask_WithoutReset_ShouldReturnNone`
- `ResetDialogCompletion_ShouldCreateFreshCompletionTask`
- `CloseDialog_ShouldCompleteCompletionTask_WithResult`
- `CloseDialog_WithoutReset_ShouldNotThrow`

------------------------------------------------------------------------
