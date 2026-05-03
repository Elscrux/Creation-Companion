using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using Serilog;
namespace CreationEditor.Services.Mutagen.References;

public sealed class ModReferenceUpdateTrigger<TCache, TLink, TSubscriber>(
    ILogger logger,
    IRecordController recordController,
    IEditorEnvironment editorEnvironment)
    : IDisposable, IReferenceUpdateTrigger<IModGetter, RecordModPair, TCache, TLink, IFormLinkIdentifier, TSubscriber>
    where TCache : IReferenceCache<TCache, TLink, IFormLinkIdentifier>
    where TLink : notnull
    where TSubscriber : IReferenced {
    private CancellationTokenSource _tokenSource = new();
    private readonly CompositeDisposable _loadOrderChangedDisposables = new();
    public IFormLinkIdentifier ToReference(RecordModPair element) => element.Record;
    public IModGetter GetSourceFor(RecordModPair element) => element.Mod;
    public void SetupSubscriptions(
        ReferenceController<IModGetter, RecordModPair, TCache, TLink, IFormLinkIdentifier, TSubscriber> referenceController,
        CompositeDisposable disposables) {
        editorEnvironment.LoadOrderChanged
            .ObserveOnTaskpool()
            .Subscribe(_ => {
                _tokenSource.Cancel();

                _loadOrderChangedDisposables.Clear();

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
                        e));

                recordController.RecordChangedDiff
                    .Subscribe(referenceController.RegisterUpdate)
                    .DisposeWith(_loadOrderChangedDisposables);
            })
            .DisposeWithComposite(disposables);
    }

    public void Dispose() => _tokenSource.Dispose();
}
