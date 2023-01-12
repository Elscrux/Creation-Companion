namespace CreationEditor.Services.Notification;

public sealed class ChainedNotifier : ANotifier {
    private readonly IEnumerator<string> _stepEnumerator;
    private readonly float _countFloat;
    private int _currentStep;

    public ChainedNotifier(INotificationService notificationService, List<string> steps)
        : base(notificationService) {
        _countFloat = steps.Count;
        _stepEnumerator = steps.GetEnumerator();
    }

    public void NextStep() {
        if (_stepEnumerator.MoveNext()) {
            _currentStep++;
            NotificationService.Notify(ID, _stepEnumerator.Current, _currentStep / _countFloat);
        } else {
            NotificationService.Stop(ID);
        }
    }
}
