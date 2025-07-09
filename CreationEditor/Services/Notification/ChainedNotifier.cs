namespace CreationEditor.Services.Notification;

public sealed class ChainedNotifier(
    INotificationService notificationService,
    params IReadOnlyList<string> steps)
    : ANotifier(notificationService), IDisposable {
    private readonly IEnumerator<string> _stepEnumerator = steps.AsEnumerable().GetEnumerator();
    private readonly float _countFloat = steps.Count;
    private int _currentStep;

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

    public void Dispose() {
        _stepEnumerator.Dispose();
    }
}
