using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using CreationEditor.Avalonia.Services.Avalonia;
using Serilog;
namespace CreationEditor.Avalonia.Services.HotKey;

/// <summary>
/// Global hotkey dispatcher. Owns all global key gestures and dispatches them on the main window.
/// </summary>
public sealed class HotkeyService(
    IMainWindowProvider mainWindowProvider,
    ILogger logger) : IHotkeyService {
    private readonly Dictionary<(Key Key, KeyModifiers Modifiers), Action> _hotkeys = [];
    private Window? _attachedWindow;

    public void Register(KeyGesture gesture, Action callback) {
        _hotkeys[ToKey(gesture)] = callback;
        AttachToWindow();
    }

    public void Unregister(KeyGesture gesture) {
        _hotkeys.Remove(ToKey(gesture));
    }

    private static (Key Key, KeyModifiers Modifiers) ToKey(KeyGesture gesture) => (gesture.Key, gesture.KeyModifiers);

    private void AttachToWindow() {
        if (_attachedWindow is not null) return;

        _attachedWindow = mainWindowProvider.Current;
        if (_attachedWindow is null) {
            mainWindowProvider.MainWindowChanged
                .Take(1)
                .Subscribe(window => {
                    _attachedWindow = window;
                    AttachToWindow();
                });
            return;
        }

        _attachedWindow.KeyDown += OnKeyDown;
        _attachedWindow.Closed += (_, _) => {
            _attachedWindow = null;
        };
    }

    private void OnKeyDown(object? sender, KeyEventArgs e) {
        if (!_hotkeys.TryGetValue((e.Key, e.KeyModifiers), out var callback)) return;

        try {
            callback();
            e.Handled = true;
        } catch (Exception exception) {
            logger.Here().Error(exception, "Failed to execute hotkey");
        }
    }
}
