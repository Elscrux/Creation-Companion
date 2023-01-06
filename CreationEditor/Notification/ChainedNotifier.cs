namespace CreationEditor.Notification;

public sealed class ChainedNotifier : ANotificationContext {
    private readonly IEnumerator<string> _stepEnumerator;
    private readonly float _countFloat;
    private int _currentStep;

    public ChainedNotifier(INotifier notifier, List<string> steps)
        : base(notifier) {
        _countFloat = steps.Count;
        _stepEnumerator = steps.GetEnumerator();
    }

    public void NextStep() {
        if (_stepEnumerator.MoveNext()) {
            _currentStep++;
            Notifier.NotifyProgress(ID, _stepEnumerator.Current, _currentStep / _countFloat);
        } else {
            Notifier.Stop(ID);
        }
    }
}
