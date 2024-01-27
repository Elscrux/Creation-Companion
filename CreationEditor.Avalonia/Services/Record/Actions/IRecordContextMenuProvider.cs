using Avalonia.Input;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public interface IRecordContextMenuProvider {
    IEnumerable<object> GetMenuItems(RecordListContext context);
    void ExecutePrimary(RecordListContext context);
    void TryToExecuteHotkey(KeyGesture keyGesture, Func<RecordListContext> contextFactory);
}
