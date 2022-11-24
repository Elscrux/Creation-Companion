using DynamicData;
using Elscrux.Notification;
namespace CreationEditor.Notification; 

// public interface INotifier {
//     public SourceCache<NotificationItem, Guid> Notifications { get; }
//
//     internal void Notify(Guid id, string message, float? progress = null);
//     internal void Progress(Guid id, float progress);
//     internal void Stop(Guid id);
// }

public interface INotifier {
    internal void Notify(Guid id, string message);
    internal void NotifyProgress(Guid id, string message, float progress);
    internal void Progress(Guid id, float progress);
    internal void Stop(Guid id, int level = 0);

    public void Subscribe(INotificationReceiver receiver);
    public void Unsubscribe(INotificationReceiver receiver);
}