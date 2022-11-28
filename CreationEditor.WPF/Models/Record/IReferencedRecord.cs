using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Models.Record;

public interface IReferencedRecord {
    [Reactive] public IMajorRecordGetter Record { get; set; }
    [Reactive] public HashSet<IFormLinkIdentifier> References { get; set; }
}