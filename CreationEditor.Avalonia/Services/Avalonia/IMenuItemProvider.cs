using System.Windows.Input;
using Avalonia.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

public interface IMenuItemProvider {
    MenuItem View(ICommand command, object? parameter = null);
    MenuItem New(ICommand command, object? parameter = null);
    MenuItem Edit(ICommand command, object? parameter = null);
    MenuItem Rename(ICommand command, object? parameter = null);
    MenuItem Duplicate(ICommand command, object? parameter = null);
    MenuItem Delete(ICommand command, object? parameter = null);
    MenuItem File(ICommand command, object? parameter = null);
    MenuItem References(ICommand command, object? parameter = null);
}
