using Avalonia.Media;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.Lifecycle;
using DynamicData.Binding;
using FluentAvalonia.UI.Windowing;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Views;

public sealed class StartupSplashScreen : ViewModel, IApplicationSplashScreen, ISplashScreenVM {
    private readonly ILifecycleStartup _lifecycleStartup;

    public IObservableCollection<string> Messages { get; } = new ObservableCollectionExtended<string>();
    public int MaxMessages { get; set; } = 6;

    [Reactive] public int Counter { get; set; }
    [Reactive] public int Total { get; set; }

    public string AppName => "Creation Companion";
    public IImage AppIcon { get; }
    public object SplashScreenContent { get; }

    public int MinimumShowTime => 0;

    public StartupSplashScreen(
        ILifecycleStartup lifecycleStartup) {
        _lifecycleStartup = lifecycleStartup;
        SplashScreenContent = new SplashScreen(this);
    }

    public async Task RunTasks(CancellationToken token) {
        await Task.Delay(1000, token);
        // using var disposable = new DisposableBucket();
        // var done = new Subject<string>();
        // done.ObserveOnGui()
        //     .Subscribe(finished => {
        //         Counter++;
        //         Messages.Add(finished);
        //         while (Messages.Count > MaxMessages) {
        //             Messages.RemoveAt(0);
        //         }
        //     })
        //     .DisposeWith(disposable);
        //
        // Total = _lifecycleStartup.LifecycleTasks.Count;
        // await Task.Run(() => _lifecycleStartup.PostStartupAsync(done, token), token);
    }
}
