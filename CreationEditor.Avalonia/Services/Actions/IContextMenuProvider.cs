using Avalonia.Input;
namespace CreationEditor.Avalonia.Services.Actions;

public interface IContextMenuProvider {
    /// <summary>
    /// Get the context menu that can be performed on a record or asset based on the context of the given list of selected elements
    /// </summary>
    /// <param name="context">Context of the selected list</param>
    /// <returns>Context menu list for actions that can be performed on a record or asset</returns>
    IEnumerable<object> GetMenuItems(SelectedListContext context);

    /// <summary>
    /// Execute the primary action on a record or asset based on the context of the selected list
    /// </summary>
    /// <param name="context">Context of the selected list</param>
    void ExecutePrimary(SelectedListContext context);

    /// <summary>
    /// Try to execute a hotkey action based on the context of the selected list
    /// </summary>
    /// <param name="keyGesture">Key gesture to execute</param>
    /// <param name="contextFactory">Context factory to create the context of the selected list</param>
    void TryToExecuteHotkey(KeyGesture keyGesture, Func<SelectedListContext> contextFactory);
}
