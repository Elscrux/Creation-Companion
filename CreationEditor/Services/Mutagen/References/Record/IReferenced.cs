using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Mutagen.References.Record;

public interface IReferenced {
    IObservableCollection<IFormLinkIdentifier> RecordReferences { get; }
}
