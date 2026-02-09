using System.IO.Hashing;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.State;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Reloaded.Memory.Extensions;
namespace CreationEditor.Avalonia.Services.Record.Prefix;

public sealed record RecordPrefixMemento(
    string Prefix);

public partial class RecordPrefixService(
    IEditorEnvironment editorEnvironment,
    IStateRepositoryFactory<RecordPrefixMemento, RecordPrefixMemento, string> stateRepositoryFactory)
    : ReactiveObject, IRecordPrefixService, ILifecycleTask {
    private readonly IStateRepository<RecordPrefixMemento, RecordPrefixMemento, string> _stateRepository = stateRepositoryFactory.Create("RecordPrefix");

    [Reactive] public partial string Prefix { get; set; } = string.Empty;

    public IObservable<string> PrefixChanged => this.WhenAnyValue(x => x.Prefix)
        .PublishRefCount();

    public void PreStartup() {
        // No work needed
    }

    public void PostStartupAsync(CancellationToken token) {
        var hash = GetLoadOrderHash();
        var recordPrefixMemento = _stateRepository.Load(hash.ToHexString());
        if (recordPrefixMemento is null) return;

        Prefix = recordPrefixMemento.Prefix;
    }

    public void OnExit() {
        var hash = GetLoadOrderHash();
        var memento = new RecordPrefixMemento(Prefix);

        _stateRepository.Save(memento, hash.ToHexString());
    }

    private byte[] GetLoadOrderHash() {
        var hashAlgorithm = new XxHash3();
        foreach (var modFileName in editorEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey.FileName)) {
            hashAlgorithm.Append(modFileName.String.AsSpan().AsBytes());
        }

        return hashAlgorithm.GetCurrentHash();
    }
}
