# Navigation Guide

This document explains navigation in **ADaxer.MvvmNav** in more detail.

---

## Basic Navigation

Navigate between ViewModels:

```csharp
await navigation.NavigateAsync<HomeViewModel>();
```

This creates the ViewModel and updates the Shell.

---

## Navigation with Parameters

```csharp
await navigation.NavigateAsync<DetailsViewModel>(
    ("Id", 42),
    ("Mode", "Edit"));
```

Read parameters in the target ViewModel:

```csharp
public class DetailsViewModel : INavigationAware
{
    public Task OnNavigatedToAsync(NavigationParameters parameters)
    {
        var id = parameters.GetValueOrDefault<int>("Id");
        return Task.CompletedTask;
    }
}
```

---

## Navigation Identity (Important)

Navigation is identified by:

- ViewModel type
- NavigationKey

Default key is based on parameters.

### Behavior

- Same type + same key → navigation blocked
- Same type + different key → navigation allowed

---

## Custom Navigation Key

Use when parameters are complex:

```csharp
await navigation.NavigateAsync<DetailsViewModel>(
    ("Customer", customer),
    options: NavigationOptions.WithKey(customer.Id.ToString()));
```

---

## Back Navigation

```csharp
await navigation.GoBackAsync();
```

Restores the previous ViewModel with parameters.

---

## Back Stack Behavior

Each entry stores:

- ViewModel instance
- Parameters
- NavigationKey

---

## Back Stack Options

```csharp
await navigation.NavigateAsync<HomeViewModel>(
    options: new NavigationOptions
    {
        ClearBackStack = true,
        AddToBackStack = true
    });
```

### Behavior

- ClearBackStack → removes history
- AddToBackStack → adds current entry

---

## Navigation Lifecycle

Implement `INavigationAware`:

```csharp
Task OnNavigatedToAsync(NavigationParameters parameters);
```

Use for:

- loading data
- initialization

---

## Navigation Guards

Prevent navigation:

```csharp
public class SettingsViewModel : ICanNavigateFrom
{
    public Task<NavigationGuardResult> CanNavigateFromAsync(
        NavigationRequest request,
        CancellationToken token)
    {
        return Task.FromResult(NavigationGuardResult.Allow());
    }
}
```

### Results

- Allow → navigate
- Disallow → stay
- AskUser → dialog

---

## AskUser Flow

```csharp
NavigationGuardResult.AskUser(
    "Unsaved changes?",
    async (result, token) =>
    {
        // continue logic
    });
```

---

## Navigation Events

`INavigationService` exposes events after:

- successful navigation
- back navigation

Not raised when:

- blocked
- cancelled

---

## Typical Flow

```text
ViewModel requests navigation
        ↓
NavigationService evaluates guards
        ↓
ViewModel created
        ↓
Shell updated
        ↓
View rendered
```

---

## Summary

- Navigation is ViewModel-first
- Parameters define identity
- Back stack is semantic
- Guards control flow
- Lifecycle hooks enable async init
