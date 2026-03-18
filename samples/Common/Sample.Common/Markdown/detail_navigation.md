## ViewModel-first 🧭 Navigation

MvvmNav uses a **ViewModel-first** approach.

### How it works
- Navigation targets **ViewModels**, not Views
- The View is resolved automatically using **DataTemplates**
- No explicit View lookup is required
- IViewAware is supported for receiving **NavigationParameters**
- Everything is Task-based for async support
- **NavigationOptions** to use or not use **BackStack** 

### Benefits

- Clean separation of concerns
- No dependency on UI types
- Easy to test