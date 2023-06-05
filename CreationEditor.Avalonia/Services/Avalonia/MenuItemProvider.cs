using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

public sealed class MenuItemProvider : IMenuItemProvider {
    public MenuItem View(ICommand command, object? parameter = null) {
        return new MenuItem {
            Icon = new SymbolIcon { Symbol = Symbol.View },
            Header = "View",
            Command = command,
            CommandParameter = parameter
        };
    }

    public MenuItem File(ICommand command, object? parameter = null) {
        return new MenuItem {
            Icon = new SymbolIcon { Symbol = Symbol.OpenFile },
            Header = "Open",
            Command = command,
            CommandParameter = parameter
        };
    }

    public MenuItem Rename(ICommand command, object? parameter = null) {
        return new MenuItem {
            Icon = new SymbolIcon { Symbol = Symbol.Rename },
            Header = "Rename",
            InputGesture = new KeyGesture(Key.F2),
            HotKey = new KeyGesture(Key.F2),
            Command = command,
            CommandParameter = parameter
        };
    }

    public MenuItem New(ICommand command, object? parameter = null) {
        return new MenuItem {
            Icon = new SymbolIcon { Symbol = Symbol.New },
            Header = "New",
            Command = command,
            CommandParameter = parameter,
        };
    }

    public MenuItem Edit(ICommand command, object? parameter = null) {
        return new MenuItem {
            Icon = new SymbolIcon { Symbol = Symbol.Edit },
            Header = "Edit",
            Command = command,
            CommandParameter = parameter,
        };
    }

    public MenuItem Duplicate(ICommand command, object? parameter = null) {
        return new MenuItem {
            Icon = new SymbolIcon { Symbol = Symbol.Share },
            Header = "Duplicate",
            Command = command,
            CommandParameter = parameter,
        };
    }

    public MenuItem Delete(ICommand command, object? parameter = null) {
        return new MenuItem {
            Icon = new SymbolIcon { Symbol = Symbol.Delete },
            Header = "Delete",
            InputGesture = new KeyGesture(Key.Delete),
            HotKey = new KeyGesture(Key.Delete),
            Command = command,
            CommandParameter = parameter,
        };
    }

    public MenuItem References(ICommand command, object? parameter = null) {
        return new MenuItem {
            Icon = new SymbolIcon { Symbol = Symbol.List },
            Header = "Open References",
            InputGesture = new KeyGesture(Key.R, KeyModifiers.Control),
            HotKey = new KeyGesture(Key.R, KeyModifiers.Control),
            Command = command,
            CommandParameter = parameter,
        };
    }
}
