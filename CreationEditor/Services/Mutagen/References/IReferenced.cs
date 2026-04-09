using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.References;

public interface IReferenced {
    IObservableCollection<DataRelativePath> AssetReferences { get; }
    IObservableCollection<IFormLinkIdentifier> RecordReferences { get; }
    IObservable<int> ReferenceCount { get; }
    bool HasReferences { get; }
    int GetReferenceCount() => AssetReferences.Count + RecordReferences.Count;
}
