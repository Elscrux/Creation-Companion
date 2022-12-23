using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Views.Record;
namespace CreationEditor.Avalonia.Services.Record.List;

public interface IRecordListFactory {
    public RecordList FromType(Type type, IRecordBrowserSettings? browserSettings = null);
}
