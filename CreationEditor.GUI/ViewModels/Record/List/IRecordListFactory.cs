using CreationEditor.GUI.Models.Record.Browser;
namespace CreationEditor.GUI.ViewModels.Record;

public interface IRecordListFactory {
    public IRecordListVM FromType(Type type, IRecordBrowserSettings? browserSettings = null);
}
