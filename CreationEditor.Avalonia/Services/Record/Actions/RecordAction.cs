using System.Reactive;
using Avalonia.Controls;
using Avalonia.Input;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public sealed record RecordAction(
    Func<RecordListContext, bool> IsVisible,
    int Priority,
    RecordActionGroup Group,
    ReactiveCommand<RecordListContext, Unit>? Command,
    Func<RecordListContext, MenuItem> MenuItemFactory,
    Func<KeyGesture?>? HotKeyFactory = null,
    bool IsPrimary = false);
