using System.Reactive.Linq;
using CreationEditor.Services.Environment;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class AutoSaveService : IAutoSaveService, IDisposable {
    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IModSaveService _modSaveService;

    private IDisposable? _onIntervalDisposable;
    private IDisposable? _onShutdownDisposable;

    private int _maxBackups = -1;

    public AutoSaveService(
        IEditorEnvironment editorEnvironment,
        IModSaveService modSaveService) {
        _editorEnvironment = editorEnvironment;
        _modSaveService = modSaveService;
    }

    private void OnInterval(double minutes) {
        _onIntervalDisposable?.Dispose();
        _onIntervalDisposable = Observable
            .Interval(TimeSpan.FromMinutes(minutes))
            .Subscribe(_ => PerformAutoSave());
    }

    private void OnShutdown() {
        _onShutdownDisposable?.Dispose();
        _onShutdownDisposable = Observable.FromEvent<EventHandler, EventArgs>(
                x => (sender, args) => x(args),
                handler => AppDomain.CurrentDomain.ProcessExit += handler,
                handler => AppDomain.CurrentDomain.ProcessExit -= handler)
            .Merge(Observable.FromEvent<UnhandledExceptionEventHandler, UnhandledExceptionEventArgs>(
                x => (sender, args) => x(args),
                handler => AppDomain.CurrentDomain.UnhandledException += handler,
                handler => AppDomain.CurrentDomain.UnhandledException -= handler))
            .Subscribe(_ => PerformAutoSave());
    }

    public void SetSettings(AutoSaveSettings settings) {
        _maxBackups = settings.MaxAutSaveCount;

        if (settings.OnInterval) {
            OnInterval(settings.IntervalInMinutes);
        } else {
            _onIntervalDisposable?.Dispose();
        }

        if (settings.OnShutdown) {
            OnShutdown();
        } else {
            _onShutdownDisposable?.Dispose();
        }
    }

    public void PerformAutoSave() {
        _modSaveService.BackupMod(_editorEnvironment.ActiveMod, _maxBackups);
        _modSaveService.SaveMod(_editorEnvironment.LinkCache, _editorEnvironment.ActiveMod);
    }

    public void Dispose() {
        _onIntervalDisposable?.Dispose();
        _onShutdownDisposable?.Dispose();
    }
}
