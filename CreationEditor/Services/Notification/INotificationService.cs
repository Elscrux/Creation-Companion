namespace CreationEditor.Services.Notification;

public interface INotificationService {
    /// <summary>
    /// Emits whenever a notification is updated.
    /// Notifications without a message are considered complete.
    /// </summary>
    IObservable<NotificationItem> Notifications { get; }

    /// <summary>
    /// Notify the user of a long running task
    /// </summary>
    /// <param name="id">Id of the task</param>
    /// <param name="message">Description of the task</param>
    /// <param name="progress">Progress of the task</param>
    internal void Notify(Guid id, string message, float? progress = null);

    /// <summary>
    /// Stops the notification of a long running task
    /// </summary>
    /// <param name="id">Id of the task</param>
    internal void Stop(Guid id);
}
