using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Record;

public interface IReferencedRecord {
    [Reactive] public IMajorRecordGetter Record { get; set; }
    public HashSet<IFormLinkIdentifier> References { get; set; }
}