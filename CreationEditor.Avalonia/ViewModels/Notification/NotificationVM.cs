using System.Reactive.Linq;
using CreationEditor.Services.Notification;
using DynamicData;
using DynamicData.Binding;
namespace CreationEditor.Avalonia.ViewModels.Notification;

public sealed class NotificationVM : ViewModel, INotificationVM {
    private readonly IObservableCollection<NotificationItem> _loadingItems;
    public IList<NotificationItem> LoadingItems => _loadingItems;

    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(500);

    public IObservable<NotificationItem> LatestNotification { get; }

    public NotificationVM(INotificationService notificationService) {
        LatestNotification = notificationService.Notifications
            .Where(x => x.LoadText is not null)
            .Sample(UpdateInterval);

        _loadingItems = notificationService.Notifications
            .ToObservableChangeSet(x => x.ID)
            .Buffer(UpdateInterval)
            .FlattenBufferResult()
            .Filter(x => x.LoadText is not null)
            .ToObservableCollection(this);
    }
}
