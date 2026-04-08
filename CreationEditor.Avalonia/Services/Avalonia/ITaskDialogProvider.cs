using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Views;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

public interface ITaskDialogProvider {
    TaskDialog CreateTaskDialog(string header, Control? content, Action<TaskDialog>? configure = null, Visual? xamlRoot = null);
}

public class TaskDialogProvider(MainWindow mainWindow) : ITaskDialogProvider {
    public TaskDialog CreateTaskDialog(string header, Control? content, Action<TaskDialog>? configure = null, Visual? xamlRoot = null) {
        var assetDialog = new TaskDialog {
            Header = header,
            Content = content,
            XamlRoot = xamlRoot ?? mainWindow,
            Buttons = {
                TaskDialogButton.OKButton,
                TaskDialogButton.CancelButton,
            },
            KeyBindings = {
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Enter),
                    Command = TaskDialogButton.OKButton.Command,
                },
                new KeyBinding {
                    Gesture = new KeyGesture(Key.Escape),
                    Command = TaskDialogButton.CancelButton.Command,
                },
            },
        };

        content?.KeyBindings.AddRange(assetDialog.KeyBindings);

        configure?.Invoke(assetDialog);

        return assetDialog;
    }
}
