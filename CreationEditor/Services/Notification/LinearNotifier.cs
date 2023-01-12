namespace CreationEditor.Services.Notification;

public sealed class LinearNotifier : ANotifier {
    private readonly float? _countFloat;
    private int _currentStep;

    /// <summary>
    /// LinearNotifier is used for simple notifications in a row.
    /// It can also provide progress information if predetermined
    /// count is set int the constructor.
    /// </summary>
    /// <param name="notificationService">NotificationService to notify</param>
    /// <param name="count">Amount of steps the notificationService will go through</param>
    /// <example>
    /// Doing tasks in a row.
    /// <code>
    ///
    /// var linearNotifier = new LinearNotifier(notificationService, 3);
    /// 
    /// linearNotifier.Next("Prepare files");
    /// PrepareFiles();
    /// 
    /// linearNotifier.Next("Generate cache");
    /// GenerateCache();
    /// 
    /// linearNotifier.Next("Finalize");
    /// Finalize(); 
    /// 
    /// linearNotifier.Stop();
    /// </code>
    /// </example>
    public LinearNotifier(INotificationService notificationService, int count = 1)
        : base(notificationService) {
        _countFloat = count;
    }

    public void Next(string message) {
        if (_countFloat is > 1) {
            NotificationService.Notify(ID, message, _currentStep / _countFloat.Value);
            _currentStep++;
        } else {
            NotificationService.Notify(ID, message);
        }
    }

    public void Stop() {
        NotificationService.Stop(ID);
    }
}
