using System.Reactive.Subjects;
namespace CreationEditor.Notification;

public sealed class NotificationService : INotificationService {
    private readonly Subject<NotificationItem> _notifications = new();
    public IObservable<NotificationItem> Notifications => _notifications;

    public void Notify(Guid id, string message, float? progress = null) {
        _notifications.OnNext(new NotificationItem(id, message, progress));
    }
    
    public void Stop(Guid id) {
        _notifications.OnNext(new NotificationItem(id));
    }
}