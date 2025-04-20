using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

public sealed class MenuItemProvider : IMenuItemProvider {
    private static MenuItem Init(MenuItem menuItem, object? parameter, KeyGesture? keyGesture = null) {
        if (parameter is IBinding binding) {
            menuItem[!MenuItem.CommandParameterProperty] = binding;
        } else {
            menuItem.CommandParameter = parameter;
        }
        menuItem.HotKey = keyGesture;
        menuItem.InputGesture = keyGesture;
        return menuItem;
    }

    public MenuItem Save(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Save },
                Header = customHeader ?? "Save",
            },
            parameter,
            new KeyGesture(Key.S, KeyModifiers.Control));
    }

    public MenuItem View(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.View },
                Header = customHeader ?? "View",
            },
            parameter);
    }

    public MenuItem File(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.OpenFile },
                Header = customHeader ?? "Open",
            },
            parameter);
    }

    public MenuItem Rename(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Rename },
                Header = customHeader ?? "Rename",
            },
            parameter,
            new KeyGesture(Key.F2));
    }

    public MenuItem New(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.New },
                Header = customHeader ?? "New",
            },
            parameter);
    }

    public MenuItem Edit(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Edit },
                Header = customHeader ?? "Edit",
            },
            parameter);
    }

    public MenuItem Duplicate(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Copy },
                Header = customHeader ?? "Duplicate",
            },
            parameter,
            new KeyGesture(Key.D, KeyModifiers.Control));
    }

    public MenuItem Delete(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Delete },
                Header = customHeader ?? "Delete",
            },
            parameter,
            new KeyGesture(Key.Delete));
    }

    public MenuItem References(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.List },
                Header = customHeader ?? "Open References",
            },
            parameter,
            new KeyGesture(Key.R, KeyModifiers.Control));
    }

    public MenuItem Copy(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Copy },
                Header = customHeader ?? "Copy",
            },
            parameter,
            new KeyGesture(Key.C, KeyModifiers.Control));
    }

    public MenuItem Paste(ICommand command, object? parameter = null, string? customHeader = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Paste },
                Header = customHeader ?? "Paste",
            },
            parameter,
            new KeyGesture(Key.V, KeyModifiers.Control));
    }

    public MenuItem Custom(ICommand command, string customHeader, object? parameter = null, object? icon = null, KeyGesture? keyGesture = null) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = icon switch {
                    Symbol symbol => new SymbolIcon { Symbol = symbol },
                    _ => icon,
                },
                Header = customHeader,
            },
            parameter,
            keyGesture);
    }
}
