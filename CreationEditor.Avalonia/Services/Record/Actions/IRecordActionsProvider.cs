namespace CreationEditor.Avalonia.Services.Record.Actions;

public interface IRecordActionsProvider {
    IEnumerable<RecordAction> GetActions();
}