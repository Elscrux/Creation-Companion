namespace CreationEditor.Notification;

public class CountingNotifier : ANotificationContext {
    private readonly float _countFloat;
    private readonly int _count;
    private int _currentStep;

    /// <summary>
    /// CountingNotifier helps with many notifications in a row of the same type.
    /// Usually used in a for-loop. CountingNotifier needs to be initialized a
    /// call to Start() where you can pass a message that will be displayed for
    /// all following steps using NextStep().
    /// </summary>
    /// <param name="notifier">Notifier to notify</param>
    /// <param name="count">Amount of steps to go through</param>
    /// <example>
    /// Iterating a list and doing something for every item in the list.
    /// <code>
    /// var counter = new CountingNotifier(notifier, list.Count);
    /// counter.Start("Doing job");
    /// foreach (var item in list) {
    ///     counter.NextStep();
    ///     // do something
    /// }
    /// counter.Stop()
    /// </code>
    /// </example>
    public CountingNotifier(INotifier notifier, int count)
        : base(notifier) {
        _countFloat = _count = count;
    }

    public void Start(string message) {
        _currentStep = 0;
        Notifier.Notify(ID, message);
    }

    public void NextStep() {
        _currentStep++;
        Notifier.Progress(ID, _currentStep / _countFloat);
        if (_currentStep == _count) {
            Notifier.Stop(ID);
        }
    }

    public void Stop() {
        Notifier.Stop(ID);
    }
}
