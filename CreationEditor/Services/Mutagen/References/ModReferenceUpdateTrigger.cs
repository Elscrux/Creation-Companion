using System.Reactive.Disposables;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References;

public sealed class ModReferenceUpdateTrigger<TCache, TLink, TSubscriber>(
    ILogger logger,
    IEditorEnvironment editorEnvironment)
    : IDisposable, IReferenceUpdateTrigger<IModGetter, RecordModPair, TCache, TLink, IFormLinkIdentifier, TSubscriber>
    where TCache : IReferenceCache<TCache, TLink, IFormLinkIdentifier>
    where TLink : notnull
    where TSubscriber : IReferenced {
    private CancellationTokenSource _tokenSource = new();
    public IFormLinkIdentifier ToReference(RecordModPair element) => element.Record;
    public IModGetter GetSourceFor(RecordModPair element) => element.Mod;
    public void SetupSubscriptions(
        ReferenceController<IModGetter, RecordModPair, TCache, TLink, IFormLinkIdentifier, TSubscriber> referenceController,
        CompositeDisposable disposables) {
        editorEnvironment.LoadOrderChanged
            .ObserveOnTaskpool()
            .Subscribe(_ => {
                _tokenSource.Cancel();

                Task.Run(async () => {
                        _tokenSource = new CancellationTokenSource();
                        await referenceController.UpdateSources(editorEnvironment.LinkCache.ListedOrder, _tokenSource.Token);
                        logger.Here().Information("Loaded all {Name} References for {Count} Mods",
                            referenceController.Name,
                            editorEnvironment.LinkCache.ListedOrder.Count);
                    })
                    .FireAndForget(e => logger.Here().Error(e,
                        "Failed to load {Name} References for {Count} Mods {Exception}",
                        referenceController.Name,
                        editorEnvironment.LinkCache.ListedOrder.Count,
                        e.Message));
            })
            .DisposeWith(disposables);
    }

    public void Dispose() => _tokenSource.Dispose();
}
