using Avalonia.Controls;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public interface IExtraColumnProvider {
    public List<DataGridColumn> GetColumns(Type type);
}
