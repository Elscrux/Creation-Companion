namespace CreationEditor.Notification;

public sealed class CountingNotifier : ANotifier {
    private readonly float _countFloat;
    private readonly int _count;
    private int _currentStep;

    private readonly string _message;

    /// <summary>
    /// CountingNotifier helps with many notifications in a row of the same type.
    /// Usually used in a for-loop. CountingNotifier needs to be initialized a
    /// call to Start() where you can pass a message that will be displayed for
    /// all following steps using NextStep().
    /// </summary>
    /// <param name="notificationService">NotificationService to notify</param>
    /// <param name="message">Message displayed per notification step</param>
    /// <param name="count">Amount of steps to go through</param>
    /// <example>
    /// Iterating a list and doing something for every item in the list.
    /// <code>
    /// var counter = new CountingNotifier(notificationService, "Doing job", list.Count);
    /// foreach (var item in list) {
    ///     counter.NextStep();
    ///     // do something
    /// }
    /// counter.Stop()
    /// </code>
    /// </example>
    public CountingNotifier(INotificationService notificationService, string message, int count)
        : base(notificationService) {
        _message = message;
        _countFloat = _count = count;
    }

    public void NextStep() {
        _currentStep++;
        NotificationService.Notify(ID, _message, _currentStep / _countFloat);
        
        if (_currentStep == _count) {
            Stop();
        }
    }

    public void Stop() {
        NotificationService.Stop(ID);
    }
}
