using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
namespace CreationEditor.Avalonia.Services.Avalonia;

public interface IMenuItemProvider {
    MenuItem Save(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem View(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem New(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem Edit(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem Rename(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem Duplicate(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem Delete(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem File(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem References(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem Copy(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem Paste(ICommand command, object? parameter = null, string? customHeader = null);
    MenuItem Custom(ICommand command, string customHeader, object? parameter = null, object? icon = null, KeyGesture? keyGesture = null);
}
