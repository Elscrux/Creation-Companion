namespace CreationEditor.Notification;

public class LinearNotifier : ANotificationContext {
    private readonly float? _countFloat;
    private int _currentStep;

    /// <summary>
    /// LinearNotifier is used for simple notifications in a row.
    /// It can also provide progress information if predetermined
    /// count is set int the constructor.
    /// </summary>
    /// <param name="notifier">Notifier to notify</param>
    /// <param name="count">Amount of steps the notifier will go through</param>
    /// <example>
    /// Doing tasks in a row.
    /// <code>
    /// var linearNotifier = new LinearNotifier(notifier, 3);
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
    public LinearNotifier(INotifier notifier, int count = 1)
        : base(notifier) {
        _countFloat = count;
    }

    public void Next(string message) {
        if (_countFloat is > 1) {
            Notifier.NotifyProgress(ID, message, _currentStep / _countFloat.Value);
            _currentStep++;
        } else {
            Notifier.Notify(ID, message);
        }
    }

    public void Stop() {
        Notifier.Stop(ID);
    }
}
