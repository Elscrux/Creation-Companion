using CreationEditor.WPF.Models.Record.Browser;
using CreationEditor.WPF.ViewModels.Record;
namespace CreationEditor.WPF.Services.Record;

public interface IRecordListFactory {
    public IRecordListVM FromType(Type type, IRecordBrowserSettings? browserSettings = null);
}
