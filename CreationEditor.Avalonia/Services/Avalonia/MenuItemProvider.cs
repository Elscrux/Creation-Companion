using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

public sealed class MenuItemProvider : IMenuItemProvider {
    private static MenuItem Init(MenuItem menuItem, object? parameter, KeyGesture? keyGesture = null) {
        menuItem.Bind(MenuItem.CommandParameterProperty, parameter);
        menuItem.HotKey = keyGesture;
        menuItem.InputGesture = keyGesture;
        return menuItem;
    }

    public MenuItem Save(ICommand command, object? parameter = null, string? customHeader = null, bool hasKeyGesture = true) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Save },
                Header = customHeader ?? "Save",
            },
            parameter,
            hasKeyGesture ? new KeyGesture(Key.S, KeyModifiers.Control) : null);
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

    public MenuItem Rename(ICommand command, object? parameter = null, string? customHeader = null, bool hasKeyGesture = true) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Rename },
                Header = customHeader ?? "Rename",
            },
            parameter,
            hasKeyGesture ? new KeyGesture(Key.F2) : null);
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

    public MenuItem Duplicate(ICommand command, object? parameter = null, string? customHeader = null, bool hasKeyGesture = true) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Copy },
                Header = customHeader ?? "Duplicate",
            },
            parameter,
            hasKeyGesture ? new KeyGesture(Key.D, KeyModifiers.Control) : null);
    }

    public MenuItem Delete(ICommand command, object? parameter = null, string? customHeader = null, bool hasKeyGesture = true) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Delete },
                Header = customHeader ?? "Delete",
            },
            parameter,
            hasKeyGesture ? new KeyGesture(Key.Delete) : null);
    }

    public MenuItem References(ICommand command, object? parameter = null, string? customHeader = null, bool hasKeyGesture = true) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.List },
                Header = customHeader ?? "Open References",
            },
            parameter,
            hasKeyGesture ? new KeyGesture(Key.R, KeyModifiers.Control) : null);
    }

    public MenuItem Copy(ICommand command, object? parameter = null, string? customHeader = null, bool hasKeyGesture = true) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Copy },
                Header = customHeader ?? "Copy",
            },
            parameter,
            hasKeyGesture ? new KeyGesture(Key.C, KeyModifiers.Control) : null);
    }

    public MenuItem Paste(ICommand command, object? parameter = null, string? customHeader = null, bool hasKeyGesture = true) {
        return Init(
            new MenuItem {
                Command = command,
                Icon = new SymbolIcon { Symbol = Symbol.Paste },
                Header = customHeader ?? "Paste",
            },
            parameter,
            hasKeyGesture ? new KeyGesture(Key.V, KeyModifiers.Control) : null);
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
