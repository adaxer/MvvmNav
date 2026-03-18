## Navigation Guards

ViewModels can control navigation using `ICanNavigateFrom`.

### Possible outcomes

- Allow navigation
- Cancel navigation
- Ask the user
  - With callback into the very ViewModel

### Typical use case

Prevent leaving a page with unsaved changes.