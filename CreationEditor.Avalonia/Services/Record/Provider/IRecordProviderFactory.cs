using CreationEditor.Services.Filter;
using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Avalonia.Services.Record.Provider;

public interface IRecordProviderFactory {
    /// <summary>
    /// Create a record provider for a list of form links
    /// </summary>
    /// <param name="identifiers">Enumerable of form links</param>
    /// <param name="browserSettings">Optionally custom record browser settings for the list</param>
    /// <returns>Record provider with the form links</returns>
    IRecordProvider FromIdentifiers(IEnumerable<IFormLinkIdentifier> identifiers, IRecordBrowserSettings? browserSettings = null);

    /// <summary>
    /// Create a record provider for all records of the given type
    /// </summary>
    /// <param name="type">Type to create a list for</param>
    /// <param name="browserSettings">Optionally custom record browser settings for the list</param>
    /// <returns>Record provider with all record of the type</returns>
    IRecordProvider FromType(Type type, IRecordBrowserSettings? browserSettings = null);
}
