using CreationEditor.Avalonia.Models.Record.Browser;
namespace CreationEditor.Avalonia.Services.Record.Browser; 

public interface IRecordBrowserGroupProvider {
    public List<RecordTypeGroup> GetRecordGroups();
}