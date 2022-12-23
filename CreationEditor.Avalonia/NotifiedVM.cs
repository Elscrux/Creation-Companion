using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.Threading;
using CreationEditor.Notification;
using DynamicData.Binding;
using Elscrux.Notification;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia;

// public class NotifiedVM : ViewModel {
//     public IObservableCollection<NotificationItem>? LoadingItems { get; }
//
//     [Reactive] public NotificationItem? LatestNotification { get; set; }
//
//     public NotifiedVM(INotifier notifier) {
//         var notificationUpdate = notifier.Notifications
//             .Connect();
//
//         LoadingItems = notificationUpdate
//             .ObserveOnGui()
//             .ToObservableCollection(this);
//
//         notificationUpdate.Subscribe(loadingItems => {
//             var addedItem = loadingItems
//                 .LastOrDefault(item => item.Reason == ChangeReason.Add)
//                 .Current;
//             
//             if (addedItem != null) LatestNotification = addedItem;
//         });
//     }
// }
public class NotifiedVM : ViewModel, INotificationReceiver {
    public ObservableCollection<NotificationItem> LoadingItems { get; } = new();

    private readonly ObservableAsPropertyHelper<bool> _anyLoading;
    public bool AnyLoading => _anyLoading.Value;

    private readonly ObservableAsPropertyHelper<NotificationItem?> _latestNotification;
    public NotificationItem? LatestNotification => _latestNotification.Value;

    public NotifiedVM() {
        var observableLoadingItems = LoadingItems.ToObservableChangeSet();

        _anyLoading = observableLoadingItems
            .Select(x => LoadingItems.Count > 0)
            .ToProperty(this, x => x.AnyLoading);

        _latestNotification = observableLoadingItems
            .Select(x => LoadingItems.LastOrDefault())
            .ToProperty(this, x => x.LatestNotification);
    }

    public void ReceiveNotify(Guid id, string message) {
        var item = LoadingItems.FirstOrDefault(item => item.ID == id);
        if (item != null) {
            item.LoadText = message;
        } else {
            Dispatcher.UIThread.Post(() => LoadingItems.Add(new NotificationItem(id, message, 0)));
        }
    }

    public void ReceiveProgress(Guid id, float progress) {
        Dispatcher.UIThread.Post(() => {
            var item = LoadingItems.FirstOrDefault(item => item.ID == id);
            if (item != null) {
                item.LoadProgress = progress;
            }
        });
    }

    public void ReceiveStop(Guid id) {
        Dispatcher.UIThread.Post(() => LoadingItems.RemoveWhere(x => x.ID == id));
    }
}
