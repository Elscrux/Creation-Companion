using Elscrux.Notification;
namespace CreationEditor.Notification;

// public class Notifier : INotifier {
//     public SourceCache<NotificationItem, Guid> Notifications { get; } = new(x => x.ID);
//
//     void INotifier.Notify(Guid id, string message, float? progress) {
//         var optional = Notifications.Lookup(id);
//         if (optional.HasValue) {
//             optional.Value.LoadText = message;
//             optional.Value.LoadProgress = progress;
//             Notifications.AddOrUpdate(optional.Value);
//         } else {
//             Notifications.AddOrUpdate(new NotificationItem(id, message));
//         }
//     }
//     
//     void INotifier.Progress(Guid id, float progress) {
//         var optional = Notifications.Lookup(id);
//         if (!optional.HasValue) return;
//
//         optional.Value.LoadProgress = progress;
//         Notifications.AddOrUpdate(optional.Value);
//     }
//     
//     void INotifier.Stop(Guid id) {
//         Notifications.Remove(id);
//     }
// }

public sealed class Notifier : INotifier {
    #region Observer
    private readonly List<INotificationReceiver> _observers = new();

    public void Subscribe(INotificationReceiver receiver) => _observers.Add(receiver);
    public void Unsubscribe(INotificationReceiver receiver) => _observers.Remove(receiver);
    #endregion
    
    #region Notification
    void INotifier.Notify(Guid id, string message) {
        _observers.ForEach(x => x.ReceiveNotify(id, message));
    }

    void INotifier.NotifyProgress(Guid id, string message, float progress) {
        _observers.ForEach(x => x.ReceiveNotify(id, message));
        _observers.ForEach(x => x.ReceiveProgress(id, progress));
    }
    
    void INotifier.Progress(Guid id, float progress) {
        _observers.ForEach(x => x.ReceiveProgress(id, progress));
    }
    
    void INotifier.Stop(Guid id, int level) {
        _observers.ForEach(x => x.ReceiveStop(id));
    }
    #endregion
}
