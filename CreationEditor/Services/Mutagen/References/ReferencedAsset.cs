using System.Reactive.Linq;
using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using ReactiveUI;
namespace CreationEditor.Services.Mutagen.References;

public sealed class ReferencedAsset : IReferencedAsset {
    public IAssetLinkGetter AssetLink { get; }
    public IObservableCollection<IFormLinkIdentifier> RecordReferences { get; }
    public IObservableCollection<DataRelativePath> AssetReferences { get; }
    public bool HasReferences => RecordReferences.Count > 0 || AssetReferences.Count > 0;
    public IObservable<int> ReferenceCount { get; }

    public ReferencedAsset(
        IAssetLinkGetter assetLink,
        IEnumerable<IFormLinkIdentifier> recordReferences,
        IEnumerable<DataRelativePath> assetReferences) {
        AssetLink = assetLink;

        RecordReferences = new ObservableCollectionExtended<IFormLinkIdentifier>(recordReferences);
        AssetReferences = new ObservableCollectionExtended<DataRelativePath>(assetReferences);

        ReferenceCount = new[] {
                this.WhenAnyValue(x => x.RecordReferences.Count),
                this.WhenAnyValue(x => x.AssetReferences.Count),
            }
            .CombineLatest()
            .Select(x => x.Sum());
    }
}
