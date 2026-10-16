using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
                FATaskDialogButton.OKButton,
                FATaskDialogButton.CancelButton,
            },
            KeyBindings = {
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Enter),
                    Command = FATaskDialogButton.OKButton.Command,
                },
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Escape),
                    Command = FATaskDialogButton.CancelButton.Command,
                },
            },
        };

        content?.KeyBindings.AddRange(assetDialog.KeyBindings);

        configure?.Invoke(assetDialog);

        return assetDialog;
    }
}
