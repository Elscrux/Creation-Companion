using Avalonia.Controls;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public interface IExtraColumnProvider {
    public IEnumerable<DataGridColumn> GetColumns(Type type);
}
