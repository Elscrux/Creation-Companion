 using System.ComponentModel;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Models.Mod.Editor;

public interface IEditableRecord<TMajorRecord> : IMajorRecord, INotifyPropertyChanged
    where TMajorRecord : class, IMajorRecord {
    void CopyTo(TMajorRecord record);
}
