# Dialogs Guide

This document explains dialog handling in **ADaxer.MvvmNav**.

---

## Basic Concept

Dialogs are **ViewModel-first**, just like navigation.

You do not open Views directly.

Instead:

```csharp
var result = await navigation.ShowDialogAsync<AboutViewModel>();
```

---

## Dialog Result

Dialogs return a `DialogResult`.

```csharp
if (result == DialogResult.True)
{
    // confirmed
}
```

Typical values:

- `True`
- `False`
- `None` (cancel)

---

## Dialog ViewModels

Dialog ViewModels usually derive from:

```csharp
DialogViewModelBase
```

To close a dialog:

```csharp
CloseDialog(DialogResult.True);
```

---

## Dialog Lifecycle

```text
ViewModel requests dialog
        ↓
DialogService creates ViewModel
        ↓
Shell displays dialog
        ↓
User interacts
        ↓
DialogResult returned
```

---

## Dialog Hosting

### Overlay (Default)

Dialogs are shown inside the Shell:

```text
Shell
 ├─ CurrentModule
 └─ CurrentDialog (overlay)
```

Advantages:

- consistent UI
- no window management
- works on all platforms

---

### Window Mode (WPF only)

WPF can optionally show dialogs as separate windows. See the [Sample App](samples.md) for how it's done.

---

## Dialog Commands (Advanced)

Dialogs can define commands dynamically via `IDialogExchange`.

Example concept:

```csharp
DialogExchangeInfo
{
    Commands = new[]
    {
        new DialogCommandInfo("OK", true, DialogResult.True),
        new DialogCommandInfo("Cancel", false, DialogResult.None)
    }
}
```

The UI renders buttons automatically.

---

## ContinueAsync Flow

Dialogs can decide whether they should close:

```csharp
ContinueAsync = async (result, token) =>
{
    if (!IsValid)
        return false;

    return true;
};
```

---

## Integration with Navigation Guards

Dialogs are often triggered by navigation guards:

```csharp
NavigationGuardResult.AskUser(
    "Unsaved changes?",
    async (result, token) =>
    {
        // decide
    });
```

---

## Platform Behavior

### WPF
- Overlay or Window

### Avalonia
- Overlay

### MAUI
- Overlay

---

## Typical Use Cases

- Confirmation dialogs
- Save / discard changes
- Login dialogs
- Wizard-like flows

---

## Summary

- Dialogs are ViewModel-first
- Use `ShowDialogAsync<T>()`
- Return `DialogResult`
- Hosted via Shell (overlay)
- Optional advanced command model
