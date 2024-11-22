using CreationEditor.Services.Notification;
namespace CreationEditor.Avalonia.ViewModels.Notification;

public interface INotificationVM {
    IList<NotificationItem> LoadingItems { get; }
    IObservable<NotificationItem> LatestNotification { get; }
}
