namespace CreationEditor.Avalonia.Services.Actions.Global;

public interface IGlobalActionsProvider {
    /// <summary>
    /// Get the global actions provided by this provider.
    /// </summary>
    /// <returns>Actions provided by this provider</returns>
    IEnumerable<GlobalAction> GetActions();
}
