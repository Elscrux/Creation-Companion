using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Services.Record.List;

public interface IRecordListFactory {
    /// <summary>
    /// Create a record list for a list of form links
    /// </summary>
    /// <param name="identifiers">Enumerable of form links</param>
    /// <param name="browserSettings">Optionally custom record browser settings for the list</param>
    /// <returns>Record list with the form links</returns>
    IRecordListVM FromIdentifiers(IEnumerable<IFormLinkIdentifier> identifiers, IRecordBrowserSettingsVM? browserSettings = null);

    /// <summary>
    /// Create a record list for all records of the given type
    /// </summary>
    /// <param name="type">Type to create a list for</param>
    /// <param name="browserSettings">Optionally custom record browser settings for the list</param>
    /// <returns>Record list with all record of the type</returns>
    IRecordListVM FromType(Type type, IRecordBrowserSettingsVM? browserSettings = null);
}
