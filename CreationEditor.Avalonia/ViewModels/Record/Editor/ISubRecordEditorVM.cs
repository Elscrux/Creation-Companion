namespace CreationEditor.Avalonia.ViewModels.Record.Editor;

public interface ISubRecordEditorVM<TRecord> : IRecordEditorVM {
    new TRecord Record { get; set; }
}
