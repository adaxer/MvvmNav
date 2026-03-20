## ℹ️ About

This page demonstrates a **simple dialog**.

Dialogs in MvvmNav are handled via the `IDialogService`.

### Key points

- Dialogs are triggered from the **ViewModel**
- The dialog ViewModel implements `IDialogController`
- Dialogs and Buttons are freely configurable
- Results are returned as a **DialogResult**

ℹ️ The buttons return a **DialogResult** True, False ,None.