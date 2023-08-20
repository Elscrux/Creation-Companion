using System.Windows.Input;
using CreationEditor.Services.Mutagen.Record;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public interface IRecordContextMenuProvider {
    static abstract Type? Type { get; }

    IRecordActionsProvider RecordActionsProvider { get; }
    ICommand PrimaryCommand { get; }
    IEnumerable<object> MenuItems { get; }
}
