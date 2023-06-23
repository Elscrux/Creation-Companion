using CreationEditor.Avalonia.Models.Record.Browser;
namespace CreationEditor.Avalonia.Services.Record.Browser;

public interface IRecordBrowserGroupProvider {
    /// <summary>
    /// Get a list of record groups
    /// </summary>
    /// <returns>List of record groups</returns>
    List<RecordTypeGroup> GetRecordGroups();
}
