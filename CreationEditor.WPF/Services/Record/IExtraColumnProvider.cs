using System.Windows.Controls;
namespace CreationEditor.WPF.Services.Record;

public interface IExtraColumnProvider {
    public List<DataGridColumn> GetColumns(Type type);
}
