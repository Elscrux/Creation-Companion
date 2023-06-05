using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Services.Record.List;

public interface IRecordListFactory {
    public IRecordListVM FromIdentifiers(IEnumerable<IFormLinkIdentifier> identifiers, IRecordBrowserSettingsVM? browserSettings = null);
    public IRecordListVM FromType(Type type, IRecordBrowserSettingsVM? browserSettings = null);
}
