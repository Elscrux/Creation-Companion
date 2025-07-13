namespace CreationEditor.Avalonia.Services.Actions;

public interface IContextActionsProvider {
    /// <summary>
    /// Get the actions that can be performed on an asset or record.
    /// </summary>
    /// <returns>Actions that can be performed on an asset or record</returns>
    IEnumerable<ContextAction> GetActions();
}
