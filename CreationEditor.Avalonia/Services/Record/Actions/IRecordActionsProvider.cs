namespace CreationEditor.Avalonia.Services.Record.Actions;

public interface IRecordActionsProvider {
    /// <summary>
    /// Get the actions that can be performed on a record
    /// </summary>
    /// <returns>Actions that can be performed on a record</returns>
    IEnumerable<RecordAction> GetActions();
}