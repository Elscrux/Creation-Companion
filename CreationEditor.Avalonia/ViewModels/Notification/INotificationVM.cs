using CreationEditor.Services.Notification;
namespace CreationEditor.Avalonia.ViewModels.Notification;

public interface INotificationVM {
    public IList<NotificationItem> LoadingItems { get; }
    public IObservable<NotificationItem> LatestNotification { get; }
}
