using Avalonia.Input;
namespace CreationEditor.Avalonia.Services.HotKey;

public interface IHotkeyService {
    /// <summary>
    /// Register a callback for a global key gesture.
    /// </summary>
    /// <param name="gesture">Key gesture to trigger the callback</param>
    /// <param name="callback">Callback to execute</param>
    void Register(KeyGesture gesture, Action callback);

    /// <summary>
    /// Unregister the callback of a global key gesture.
    /// </summary>
    /// <param name="gesture">Key gesture to unregister</param>
    void Unregister(KeyGesture gesture);
}
