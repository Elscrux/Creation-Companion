using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.Views.Record;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Services.Record.List;

public interface IRecordListFactory {
    public RecordList FromIdentifiers(IEnumerable<IFormLinkIdentifier> identifiers, IRecordBrowserSettingsVM? browserSettings = null);
    public RecordList FromType(Type type, IRecordBrowserSettingsVM? browserSettings = null);
}
