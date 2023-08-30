using System.Reactive.Linq;
namespace CreationEditor.Services.Notification;

public sealed class CountingNotifier : ANotifier, IDisposable {
    private int _currentStep;

    private readonly IDisposable _timerSubscription;


    /// <inheritdoc cref="CountingNotifier(CreationEditor.Services.Notification.INotificationService,string,int)"/>
    /// <param name="timeSpan">Time span in which the current step will be reported</param>
    public CountingNotifier(INotificationService notificationService, string message, int count, TimeSpan timeSpan)
        : base(notificationService) {
        var countFloat = (float) count;

        _timerSubscription = Observable.Interval(timeSpan)
            .TakeWhile(_ => _currentStep < count)
            .Subscribe(_ => NotificationService.Notify(ID, message, _currentStep / countFloat));
    }

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
    public CountingNotifier(INotificationService notificationService, string message, int count) : this(notificationService, message, count, TimeSpan.FromMilliseconds(100)) {}

    public void NextStep() => Interlocked.Increment(ref _currentStep);

    public void Stop() {
        _timerSubscription.Dispose();
        NotificationService.Stop(ID);
    }

    public void Dispose() => Stop();
}
