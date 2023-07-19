using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Services.Mutagen.References.Asset;

public interface IReferencedAsset {
    ModKey ModKey { get; }
    IAssetLinkGetter AssetLink { get; }
    IObservableCollection<IFormLinkGetter> RecordReferences { get; }
    IEnumerable<string> NifReferences { get; }
    IObservableCollection<string> NifDirectoryReferences { get; }
    IObservableCollection<string> NifArchiveReferences { get; }

    bool HasReferences { get; }
    IObservable<int> ReferenceCount { get; }
}
