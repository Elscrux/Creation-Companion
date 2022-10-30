using CreationEditor.GUI.Models.Record.Browser;
using CreationEditor.GUI.ViewModels.Record;
namespace CreationEditor.GUI.Services.Record;

public interface IRecordListFactory {
    public IRecordListVM FromType(Type type, IRecordBrowserSettings? browserSettings = null);
}
