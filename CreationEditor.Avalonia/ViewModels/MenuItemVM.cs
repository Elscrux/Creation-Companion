using System.Reactive;
using Avalonia.Input;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels;

public sealed class MenuItemVM(
    string header,
    IReadOnlyList<MenuItemVM> children,
    object? icon = null,
    KeyGesture? hotKey = null,
    ReactiveCommand<Unit, Unit>? openCommand = null) {
    public string Header { get; } = header;
    public IReadOnlyList<MenuItemVM> Children { get; } = children;
    public object? Icon { get; } = icon;
    public KeyGesture? HotKey { get; } = hotKey;
    public string? InputGesture => HotKey?.ToString();
    public ReactiveCommand<Unit, Unit>? OpenCommand { get; } = openCommand;
}
