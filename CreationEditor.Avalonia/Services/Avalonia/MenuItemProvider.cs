using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

public sealed class MenuItemProvider : IMenuItemProvider {
    private static MenuItem AddParameter(MenuItem menuItem, object? parameter) {
        if (parameter is IBinding binding) {
            menuItem[!MenuItem.CommandParameterProperty] = binding;
        } else {
            menuItem.CommandParameter = parameter;
        }
        return menuItem;
    }

    public MenuItem View(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.View },
                Header = customHeader ?? "View",
                Command = command,
            },
            parameter);
    }

    public MenuItem File(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.OpenFile },
                Header = customHeader ?? "Open",
                Command = command,
            },
            parameter);
    }

    public MenuItem Rename(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Rename },
                Header = customHeader ?? "Rename",
                InputGesture = new KeyGesture(Key.F2),
                HotKey = new KeyGesture(Key.F2),
                Command = command,
            },
            parameter);
    }

    public MenuItem New(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.New },
                Header = customHeader ?? "New",
                Command = command,
            },
            parameter);
    }

    public MenuItem Edit(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Edit },
                Header = customHeader ?? "Edit",
                Command = command,
            },
            parameter);
    }

    public MenuItem Duplicate(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Share },
                Header = customHeader ?? "Duplicate",
                Command = command,
            },
            parameter);
    }

    public MenuItem Delete(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Delete },
                Header = customHeader ?? "Delete",
                InputGesture = new KeyGesture(Key.Delete),
                HotKey = new KeyGesture(Key.Delete),
                Command = command,
            },
            parameter);
    }

    public MenuItem References(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.List },
                Header = customHeader ?? "Open References",
                InputGesture = new KeyGesture(Key.R, KeyModifiers.Control),
                HotKey = new KeyGesture(Key.R, KeyModifiers.Control),
                Command = command,
            },
            parameter);
    }

    public MenuItem Copy(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Copy },
                Header = customHeader ?? "Copy",
                InputGesture = new KeyGesture(Key.C, KeyModifiers.Control),
                HotKey = new KeyGesture(Key.C, KeyModifiers.Control),
                Command = command,
            },
            parameter);
    }

    public MenuItem Paste(ICommand command, object? parameter = null, string? customHeader = null) {
        return AddParameter(
            new MenuItem {
                Icon = new SymbolIcon { Symbol = Symbol.Paste },
                Header = customHeader ?? "Paste",
                InputGesture = new KeyGesture(Key.V, KeyModifiers.Control),
                HotKey = new KeyGesture(Key.V, KeyModifiers.Control),
                Command = command,
            },
            parameter);
    }
}
