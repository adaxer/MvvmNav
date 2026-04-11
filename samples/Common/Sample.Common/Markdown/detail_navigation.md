## ViewModel-first 🧭 Navigation

MvvmNav uses a **ViewModel-first** approach.

### How it works
- Navigation targets **ViewModels**, not Views
- The View is resolved automatically using **DataTemplates**
- In Maui, there is a ViewLocator to register ViewModel/View-Pairs
- No explicit View lookup is required
- `INavigationAware.OnNavigatedToAsync` is overridable for receiving `NavigationParameters`
- Everything is Task-based for async support
- `NavigationOptions` to use or not use **BackStack**
- `NavigationKey` to facilitate decision to navigate or not

### Benefits

- Clean separation of concerns
- No dependency on UI types
- Easy to test