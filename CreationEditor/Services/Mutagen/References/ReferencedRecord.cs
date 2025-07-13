using System.Reactive.Linq;
using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Services.Mutagen.References;

public sealed partial class ReferencedRecord<TMajorRecordGetter> : ReactiveObject, IReferencedRecord<TMajorRecordGetter>
    where TMajorRecordGetter : IMajorRecordIdentifierGetter {

    [Reactive] public partial TMajorRecordGetter Record { get; set; }

    public IObservableCollection<DataRelativePath> AssetReferences { get; }
    public IObservableCollection<IFormLinkIdentifier> RecordReferences { get; }
    public IObservable<int> ReferenceCount { get; } 
    public bool HasReferences => RecordReferences.Count > 0 || AssetReferences.Count > 0;

    public ReferencedRecord(
        TMajorRecordGetter record,
        IEnumerable<IFormLinkIdentifier> recordReferences,
        IEnumerable<DataRelativePath> assetReferences) {
        Record = record;
        RecordReferences =  new ObservableCollectionExtended<IFormLinkIdentifier>(recordReferences);
        AssetReferences = new ObservableCollectionExtended<DataRelativePath>(assetReferences);

        ReferenceCount = new[] {
                this.WhenAnyValue(x => x.RecordReferences.Count),
                this.WhenAnyValue(x => x.AssetReferences.Count),
            }
            .CombineLatest()
            .Select(x => x.Sum());
    }

    public override bool Equals(object? obj) {
        return obj switch {
            IReferencedRecord<TMajorRecordGetter> referencedRecord => referencedRecord.Equals(this),
            TMajorRecordGetter record => record.FormKey.Equals(Record.FormKey),
            IFormKeyGetter formKeyGetter => formKeyGetter.FormKey.Equals(Record.FormKey),
            _ => false,
        };
    }

    public override int GetHashCode() {
        // While the record can change, its form key will always stay the same
        return HashCode.Combine(Record.FormKey);
    }
}
