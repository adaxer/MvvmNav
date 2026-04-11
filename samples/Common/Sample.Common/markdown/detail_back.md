## Back Navigation 🔙

MvvmNav provides built-in back navigation with full stack control.

### How it works
- Navigation entries are stored in a back stack
- Each entry includes ViewModel type, parameters and NavigationKey
- `GoBackAsync()` restores the previous target with original parameters
- Navigation guards are evaluated before navigating back
- Platform-specific back navigation (e.g. mobile back button) is supported

### Behavior
- `ClearBackStack` resets navigation history
- `AddToBackStack` controls whether entries are stored
- `NavigationStateChanged` is raised after successful back navigation

### Benefits
- Predictable and consistent navigation flow
- Full control over navigation history
- Seamless integration with guards and dialogs