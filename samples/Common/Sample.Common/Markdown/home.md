## MvvmNav SampleApp

This sample demonstrates **MvvmNav** in a small cross-platform application.

- **ViewModel-first navigation**
- **ShellView** and **ShellViewModel** only need to implement the required interfaces
- The **NavigationService** is injected into the `ShellViewModel`
- Navigation stays fully **ViewModel-first**
- Views are resolved using native **DataTemplates**

ℹ️ `HomeViewModel` is shown when the app starts. 
ℹ️ It is an example of a plain ViewModel with just `INotifyPropertyChanged` and `INavigationAware`.

👉 Use the menu to explore dialogs, simple navigation, and framework features.