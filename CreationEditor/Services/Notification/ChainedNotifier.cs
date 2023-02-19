namespace CreationEditor.Services.Notification;

public sealed class ChainedNotifier : ANotifier {
    private readonly IEnumerator<string> _stepEnumerator;
    private readonly float _countFloat;
    private int _currentStep;

    public ChainedNotifier(INotificationService notificationService, params string[] steps)
        : base(notificationService) {
        _countFloat = steps.Length;
        _stepEnumerator = steps.AsEnumerable().GetEnumerator();
    }

    public void NextStep() {
        if (_stepEnumerator.MoveNext()) {
            _currentStep++;
            NotificationService.Notify(ID, _stepEnumerator.Current, _currentStep / _countFloat);
        } else {
            NotificationService.Stop(ID);
        }
    }

    public void Stop() {
        NotificationService.Stop(ID);
    }
}
