## Settings

This page demonstrates **simple navigation**, meaning it shows a screen without passing parameters.

Navigation is performed using the `NavigationService`.

### What happens

- A ViewModel is requested
- The view is resolved automatically via **DataTemplates**
- The current ViewModel is pushed onto the **back stack**
- Use the **back** button to return.

ℹ️ If you change data above, a dialog will ask you to keep them (so it uses a **dirty** flag) 

👉 And this works, because SettingsViewModel is registered as a **singleton** in the `AppBootstrapper` (thus keeping its values between navigations).