using System.Reactive;
using Avalonia.Input;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Actions.Global;

/// <summary>
/// Defines a globally available editor action.
/// </summary>
/// <param name="Id">Unique, stable identifier of the action</param>
/// <param name="Title">Display title, matched by the search</param>
/// <param name="Command">Command executed when the action is selected. CanExecute is honored for the enabled state</param>
/// <param name="Subtitle">Optional secondary display text</param>
/// <param name="Icon">Optional icon</param>
/// <param name="Priority">Sort weight within equal match scores. Higher is more prominent</param>
/// <param name="HotKey">Optional global hotkey that triggers the action</param>
public sealed record GlobalAction(
    string Id,
    string Title,
    ReactiveCommand<Unit, Unit>? Command,
    string? Subtitle = null,
    object? Icon = null,
    int Priority = 0,
    KeyGesture? HotKey = null);
