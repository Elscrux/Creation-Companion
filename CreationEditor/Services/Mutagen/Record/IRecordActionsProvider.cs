using System.Reactive;
using ReactiveUI;
namespace CreationEditor.Services.Mutagen.Record;

public interface IRecordActionsProvider {
    // Well known actions
    ReactiveCommand<System.Type?, Unit> New { get; }
    ReactiveCommand<object?, Unit> Edit { get; }
    ReactiveCommand<object?, Unit> Duplicate { get; }
    ReactiveCommand<object?, Unit> Delete { get; }
    ReactiveCommand<object?, Unit> OpenReferences { get; }
}
