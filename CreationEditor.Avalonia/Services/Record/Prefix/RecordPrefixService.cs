using System.IO.Hashing;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Lifecycle;
using CreationEditor.Services.State;
using Noggog;
using Reloaded.Memory.Extensions;
namespace CreationEditor.Avalonia.Services.Record.Prefix;

public sealed record RecordPrefixMemento(
    string Prefix);

public class RecordPrefixService(
    IEditorEnvironment editorEnvironment,
    IStateRepositoryFactory<RecordPrefixMemento, RecordPrefixMemento, string> stateRepositoryFactory)
    : IRecordPrefixService, ILifecycleTask {
    private readonly IStateRepository<RecordPrefixMemento, RecordPrefixMemento, string> _stateRepository = stateRepositoryFactory.Create("RecordPrefix");

    public string Prefix { get; set; } = string.Empty;

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
