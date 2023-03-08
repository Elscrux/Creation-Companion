namespace CreationEditor.Services.Notification;

public interface INotificationService {
    public IObservable<NotificationItem> Notifications { get; }

    internal void Notify(Guid id, string message, float? progress = null);
    internal void Stop(Guid id);
}
