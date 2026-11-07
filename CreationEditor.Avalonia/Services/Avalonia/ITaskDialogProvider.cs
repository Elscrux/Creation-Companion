using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

public interface ITaskDialogProvider {
    FATaskDialog CreateTaskDialog(string header, Control? content, Action<FATaskDialog>? configure = null, Visual? xamlRoot = null);
}

public sealed class TaskDialogProvider(IMainWindowProvider mainWindowProvider) : ITaskDialogProvider {
    public FATaskDialog CreateTaskDialog(string header, Control? content, Action<FATaskDialog>? configure = null, Visual? xamlRoot = null) {
        var assetDialog = new FATaskDialog {
            Header = header,
            Content = content,
            XamlRoot = xamlRoot ?? mainWindowProvider.Current,
            Buttons = {
                new FATaskDialogButton("OK", FATaskDialogStandardResult.OK) { IsDefault = true },
                new FATaskDialogButton("Cancel", FATaskDialogStandardResult.Cancel),
            },
        };

        configure?.Invoke(assetDialog);

        // Focus the first interactable element in the content once it's attached to the visual tree
        if (content is not null) {
            content.AttachedToVisualTree += (_, _) => FocusFirstTextBox(content);
        }

        return assetDialog;
    }

    private static void FocusFirstTextBox(Control content) {
        var focusable = FindFirstTextBox(content);
        if (focusable is null) return;

        if (focusable.IsAttachedToVisualTree()) {
            focusable.Focus();
        } else {
            focusable.AttachedToVisualTree += (_, _) => focusable.Focus();
        }
    }

    private static TextBox? FindFirstTextBox(Control control) {
        if (control is TextBox { Focusable: true, IsEnabled: true, IsVisible: true } textBox) return textBox;

        foreach (var child in control.GetVisualChildren().Concat(control.GetLogicalChildren()).OfType<Control>()) {
            var result = FindFirstTextBox(child);
            if (result is not null) return result;
        }

        return null;
    }
}
