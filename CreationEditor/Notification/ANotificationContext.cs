namespace CreationEditor.Notification; 

public abstract class ANotificationContext {
    protected readonly INotifier Notifier;

    protected readonly Guid ID;

    protected ANotificationContext(INotifier notifier) {
        Notifier = notifier;
        ID = Guid.NewGuid();
    }
}
