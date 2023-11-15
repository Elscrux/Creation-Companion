using System.Reactive.Linq;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using ReactiveUI;
namespace CreationEditor.Services.Mutagen.References.Asset;

public sealed class ReferencedAsset : IReferencedAsset {
    public ModKey ModKey { get; } = ModKey.Null;
    public IAssetLinkGetter AssetLink { get; }
    public IObservableCollection<IFormLinkGetter> RecordReferences { get; }
    public IEnumerable<string> NifReferences => NifDirectoryReferences.Concat(NifArchiveReferences);
    public IObservableCollection<string> NifDirectoryReferences { get; }
    public IObservableCollection<string> NifArchiveReferences { get; }
    public bool HasReferences => RecordReferences.Count > 0 || NifDirectoryReferences.Count > 0 || NifArchiveReferences.Count > 0;
    public IObservable<int> ReferenceCount { get; }

    public ReferencedAsset(
        IAssetLinkGetter assetLink, IEnumerable<IFormLinkGetter>? recordReferences, IEnumerable<string>? nifDirectoryReferences, IEnumerable<string>? nifArchiveReferences) {
        AssetLink = assetLink;

        RecordReferences = recordReferences is null
            ? []
            : new ObservableCollectionExtended<IFormLinkGetter>(recordReferences);

        NifDirectoryReferences = nifDirectoryReferences is null
            ? []
            : new ObservableCollectionExtended<string>(nifDirectoryReferences);

        NifArchiveReferences = nifArchiveReferences is null
            ? []
            : new ObservableCollectionExtended<string>(nifArchiveReferences);

        ReferenceCount = new[] {
                this.WhenAnyValue(x => x.RecordReferences.Count),
                this.WhenAnyValue(x => x.NifDirectoryReferences.Count),
                this.WhenAnyValue(x => x.NifArchiveReferences.Count),
            }
            .CombineLatest()
            .Select(x => x.Sum());
    }
}
