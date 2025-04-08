using DynamicData.Binding;
using Mutagen.Bethesda.Assets;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.References.Record;

public interface IReferenced {
    IObservableCollection<DataRelativePath> AssetReferences { get; }
    IObservableCollection<IFormLinkIdentifier> RecordReferences { get; }
}
