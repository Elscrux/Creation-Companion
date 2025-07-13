using System.Reactive;
using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Actions;

public sealed record ContextAction(
    Func<SelectedListContext, bool> IsVisible,
    int Priority,
    ContextActionGroup Group,
    ReactiveCommand<SelectedListContext, Unit>? Command,
    Func<SelectedListContext, MenuItem> MenuItemFactory,
    Func<KeyGesture?>? HotKeyFactory = null,
    bool IsPrimary = false);
