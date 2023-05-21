using DynamicData.Binding;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record;

public interface IReferenced {
    public IObservableCollection<IFormLinkIdentifier> References { get; }
}

public interface IReferencedRecordIdentifier : IReferenced, IFormLinkIdentifier {
    public IMajorRecordIdentifier Record { get; set; }
}
