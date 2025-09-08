using System.Reactive.Linq;
using CreationEditor.Services.Notification;
using DynamicData;
using DynamicData.Binding;
namespace CreationEditor.Avalonia.ViewModels.Notification;

public sealed class NotificationVM : ViewModel, INotificationVM {
    private readonly IObservableCollection<NotificationItem> _loadingItems;
    public IList<NotificationItem> LoadingItems => _loadingItems;

    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(500);

    private readonly SourceCache<NotificationItem, Guid> _currentNotifications = new(x => x.Id);

    public IObservable<NotificationItem?> LatestNotification => _currentNotifications.Connect()
        .ToCollection()
        .Select(x => x.FirstOrDefault());

    public NotificationVM(INotificationService notificationService) {
        notificationService.Notifications
            .Subscribe(notification => {
                if (notification.IsDone) {
                    _currentNotifications.Remove(notification);
                } else {
                    _currentNotifications.AddOrUpdate(notification);
                }
            });

        _loadingItems = notificationService.Notifications
            .ToObservableChangeSet(x => x.Id)
            .Buffer(UpdateInterval)
            .FlattenBufferResult()
            .Filter(x => x.IsLive)
            .ToObservableCollection(this);
    }
}
