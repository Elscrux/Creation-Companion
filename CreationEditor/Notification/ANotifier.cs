namespace CreationEditor.Notification; 

public abstract class ANotifier {
    protected readonly INotificationService NotificationService;

    protected readonly Guid ID;

    protected ANotifier(INotificationService notificationService) {
        NotificationService = notificationService;
        ID = Guid.NewGuid();
    }
}
