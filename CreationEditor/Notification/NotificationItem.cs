using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Notification;

public sealed class NotificationItem : ReactiveObject {
    public Guid ID { get; }
    // todo enable when this is fixed https://github.com/AvaloniaUI/Avalonia/issues/8810
    // [Reactive]
    public string LoadText { get; set; }
    // [Reactive] 
    public float? LoadProgress { get; set; }
    
    public NotificationItem(Guid id, string loadText, float? loadProgress = null) {
        ID = id;
        LoadText = loadText;
        LoadProgress = loadProgress;
    }
}
