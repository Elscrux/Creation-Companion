using Avalonia;
using Avalonia.Controls;
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

        return assetDialog;
    }
}
