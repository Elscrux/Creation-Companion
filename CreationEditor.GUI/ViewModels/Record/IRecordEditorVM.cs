using System.Collections.ObjectModel;
using CreationEditor.Operations;
namespace CreationEditor.GUI.ViewModels.Record; 

public interface IRecordEditorVM {
    public ObservableCollection<IOperation> Operations { get; } 

    public void Save();
}
