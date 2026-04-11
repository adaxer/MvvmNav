## ℹ️ About

This page demonstrates a **simple dialog**.

Dialogs in MvvmNav are handled via the `IDialogService`.

### Key points

- Dialogs are triggered from the **ViewModel**
- The dialog ViewModel implements `IDialogController`
- Dialogs and Buttons are freely configurable
- Results are returned as a `DialogResult` True, False, None, and generic variants with a payload

ℹ️ The dialogs are shown as **Overlay**, and in Wpf as Modal windows if desired.