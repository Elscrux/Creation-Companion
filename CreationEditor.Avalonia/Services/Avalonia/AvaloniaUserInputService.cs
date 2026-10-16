using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using CreationEditor.Services.UserInput;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

/// <summary>
/// <see cref="IUserInputService"/> implementation showing a modal dialog with a text box (backed by <see cref="ITaskDialogProvider"/>).
/// </summary>
public sealed class AvaloniaUserInputService(ITaskDialogProvider taskDialogProvider) : IUserInputService {
    public async Task<string?> RequestTextAsync(string title, string message, string? defaultValue = null, CancellationToken token = default) {
        var messageText = new TextBlock {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 0, 0, 8),
        };

        var textBox = new TextBox {
            Text = defaultValue,
            PlaceholderText = "Enter value",
            MinWidth = 320,
        };

        var content = new StackPanel {
            Orientation = Orientation.Vertical,
            Children = {
                messageText,
                textBox,
            },
        };

        var dialog = taskDialogProvider.CreateTaskDialog(title, content);
        var result = await dialog.ShowAsync();

        if (result is not FATaskDialogStandardResult.OK) {
            return null;
        }

        return string.IsNullOrWhiteSpace(textBox.Text) ? null : textBox.Text;
    }
}
