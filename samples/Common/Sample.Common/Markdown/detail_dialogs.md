## Dialog Integration

Dialogs are handled via the `IDialogService`.

### How it works

- A dialog ViewModel implements `IDialogController`
- The dialog is opened from another ViewModel
- Dialog content is resolved via DataTemplates (being a ViewModel)
- A `DialogResult` is returned
- Flexible handling of Commands (e.g. OK/Cancel, Yes/No/Cancel)

### Benefits

- No code-behind required
- Fully testable
- Consistent dialog handling