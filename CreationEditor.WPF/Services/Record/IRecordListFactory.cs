using CreationEditor.WPF.Models.Record.Browser;
using CreationEditor.WPF.Views.Record;
namespace CreationEditor.WPF.Services.Record;

public interface IRecordListFactory {
    public RecordList FromType(Type type, IRecordBrowserSettings? browserSettings = null);
}
