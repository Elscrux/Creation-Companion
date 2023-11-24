using System.Windows.Input;
using CreationEditor.Services.Mutagen.Record;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public interface IRecordContextMenuProvider {
    IRecordActionsProvider RecordActionsProvider { get; }
    ICommand PrimaryCommand { get; }
    IEnumerable<object> MenuItems { get; }
}
