using System.Reactive.Linq;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveMarbles.ObservableEvents;
namespace CreationEditor.Services.Mutagen.Mod.Save;

public sealed class AutoSaveService(
    IEditorEnvironment editorEnvironment,
    IModSaveService modSaveService)
    : IModSaveService, IAutoSaveService, IDisposable {

    private IDisposable? _onIntervalDisposable;
    private IDisposable? _onShutdownDisposable;

    private int _maxBackups = -1;

    private void OnInterval(double minutes) {
        _onIntervalDisposable?.Dispose();
        _onIntervalDisposable = Observable
            .Interval(TimeSpan.FromMinutes(minutes))
            .Subscribe(SaveMods);
    }

    private void OnShutdown() {
        _onShutdownDisposable?.Dispose();
        _onShutdownDisposable = AppDomain.CurrentDomain.Events().ProcessExit.Unit()
            .Merge(AppDomain.CurrentDomain.Events().UnhandledException.Unit())
            .Subscribe(SaveMods);
    }

    private void SaveMods() {
        foreach (var mutableMods in editorEnvironment.MutableMods) {
            SaveMod(mutableMods);
        }
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

    public void SaveMod(IMod mod) {
        modSaveService.BackupMod(mod, _maxBackups);
        modSaveService.SaveMod(mod);
    }

    public void BackupMod(IMod mod, int limit = -1) {
        modSaveService.BackupMod(mod, _maxBackups);
    }

    public void Dispose() {
        _onIntervalDisposable?.Dispose();
        _onShutdownDisposable?.Dispose();
    }
}
