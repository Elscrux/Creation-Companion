using Avalonia.Input;
namespace CreationEditor.Avalonia.Services.Record.Actions;

public interface IRecordContextMenuProvider {
    /// <summary>
    /// Get the context menu that can be performed on a record based on the context of the record list
    /// </summary>
    /// <param name="context">Context of the record list</param>
    /// <returns>Context menu list for actions that can be performed on a record</returns>
    IEnumerable<object> GetMenuItems(RecordListContext context);

    /// <summary>
    /// Execute the primary action on a record based on the context of the record list
    /// </summary>
    /// <param name="context">Context of the record list</param>
    void ExecutePrimary(RecordListContext context);

    /// <summary>
    /// Try to execute a hotkey action based on the context of the record list
    /// </summary>
    /// <param name="keyGesture">Key gesture to execute</param>
    /// <param name="contextFactory">Context factory to create the context of the record list</param>
    void TryToExecuteHotkey(KeyGesture keyGesture, Func<RecordListContext> contextFactory);
}
