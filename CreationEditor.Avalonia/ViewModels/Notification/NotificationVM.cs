using System.Reactive.Linq;
using CreationEditor.Extension;
using CreationEditor.Services.Notification;
using DynamicData;
using DynamicData.Binding;
namespace CreationEditor.Avalonia.ViewModels.Notification;

public interface INotificationVM {
    public IList<NotificationItem> LoadingItems { get; }
    public IObservable<NotificationItem> LatestNotification { get; }
}

public sealed class NotificationVM : ViewModel, INotificationVM {
    private readonly IObservableCollection<NotificationItem> _loadingItems;
    public IList<NotificationItem> LoadingItems => _loadingItems;

    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(500);

    public IObservable<NotificationItem> LatestNotification { get; }

    public NotificationVM(INotificationService notificationService) {
        LatestNotification = notificationService.Notifications
            .Where(x => x.LoadText != null)
            .Sample(UpdateInterval);
        
        _loadingItems = notificationService.Notifications
            .ToObservableChangeSet(x => x.ID)
            .Filter(x => x.LoadText != null)
            .Buffer(UpdateInterval)
            .FlattenBufferResult()
            .ToObservableCollection(this);

    }
}