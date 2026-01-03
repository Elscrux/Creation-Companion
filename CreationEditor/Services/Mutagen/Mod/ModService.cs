using System.Collections.Concurrent;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Plugins;
using Noggog;
namespace CreationEditor.Services.Mutagen.Mod;

public sealed class ModService : IModService, IDisposable {
    private readonly DisposableBucket _disposableBucket = new();
    private readonly ConcurrentDictionary<ModKey, HashSet<ModKey>> _mastersCache = new();

    private readonly IEditorEnvironment _editorEnvironment;
    private readonly IModInfoProvider _modInfoProvider;

    public ModService(
        IEditorEnvironment editorEnvironment,
        IModInfoProvider modInfoProvider) {
        _editorEnvironment = editorEnvironment;
        _modInfoProvider = modInfoProvider;

        _editorEnvironment.LinkCacheChanged
            .Subscribe(_ => _mastersCache.Clear())
            .DisposeWith(_disposableBucket);
    }

    public bool HasMasterTransitive(ModKey modKey, ModKey master) {
        if (_mastersCache.TryGetValue(modKey, out var cachedMasters)) {
            return cachedMasters.Contains(master);
        }

        var masterInfos = _modInfoProvider.GetMasterInfos(_editorEnvironment.LinkCache);
        if (masterInfos.TryGetValue(modKey, out var result) && result.Valid) {
            _mastersCache[modKey] = result.Masters;
            return result.Masters.Contains(master);
        }

        return false;
    }

    public bool IsModOrHasMasterTransitive(ModKey modKey, ModKey master) {
        return modKey.Equals(master) || HasMasterTransitive(modKey, master);
    }

    public bool TryGetMastersTransitive(ModKey modKey, out HashSet<ModKey> masters) {
        if (_mastersCache.TryGetValue(modKey, out var cachedMasters)) {
            masters = cachedMasters;
            return true;
        }

        var masterInfos = _modInfoProvider.GetMasterInfos(_editorEnvironment.LinkCache);
        if (masterInfos.TryGetValue(modKey, out var result) && result.Valid) {
            _mastersCache[modKey] = result.Masters;
            masters = result.Masters;
            return true;
        }

        masters = [];
        return false;
    }

    public void Dispose() {
        _disposableBucket.Dispose();
    }
}
