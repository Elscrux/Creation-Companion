﻿using System.Reactive.Linq;
using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using ReactiveUI;
namespace CreationEditor.Services.Mutagen.References.Asset;

public sealed class ReferencedAsset : IReferencedAsset {
    public ModKey ModKey { get; } = ModKey.Null;
    public IAssetLinkGetter AssetLink { get; }
    public IObservableCollection<IFormLinkIdentifier> RecordReferences { get; }
    public IObservableCollection<DataRelativePath> AssetReferences { get; }
    public IObservableCollection<DataRelativePath> NifDirectoryReferences { get; }
    public IObservableCollection<DataRelativePath> NifArchiveReferences { get; }
    public bool HasReferences => RecordReferences.Count > 0 || NifDirectoryReferences.Count > 0 || NifArchiveReferences.Count > 0;
    public IObservable<int> ReferenceCount { get; }

    public ReferencedAsset(
        IAssetLinkGetter assetLink,
        IEnumerable<IFormLinkIdentifier>? recordReferences,
        IEnumerable<DataRelativePath>? nifDirectoryReferences,
        IEnumerable<DataRelativePath>? nifArchiveReferences) {
        AssetLink = assetLink;

        RecordReferences = recordReferences is null
            ? []
            : new ObservableCollectionExtended<IFormLinkIdentifier>(recordReferences);

        NifDirectoryReferences = nifDirectoryReferences is null
            ? []
            : new ObservableCollectionExtended<DataRelativePath>(nifDirectoryReferences);

        NifArchiveReferences = nifArchiveReferences is null
            ? []
            : new ObservableCollectionExtended<DataRelativePath>(nifArchiveReferences);

        AssetReferences = new ObservableCollectionExtended<DataRelativePath>(NifDirectoryReferences.Concat(NifArchiveReferences));

        ReferenceCount = new[] {
                this.WhenAnyValue(x => x.RecordReferences.Count),
                this.WhenAnyValue(x => x.NifDirectoryReferences.Count),
                this.WhenAnyValue(x => x.NifArchiveReferences.Count),
            }
            .CombineLatest()
            .Select(x => x.Sum());

        NifDirectoryReferences.WhenCollectionChanges()
            .Merge(NifArchiveReferences.WhenCollectionChanges())
            .Subscribe(_ => {
                AssetReferences.Load(NifDirectoryReferences.Concat(NifArchiveReferences));
            });
    }
}
