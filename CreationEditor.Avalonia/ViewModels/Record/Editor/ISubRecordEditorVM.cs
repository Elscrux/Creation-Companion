namespace CreationEditor.Avalonia.ViewModels.Record.Editor;

public interface ISubRecordEditorVM<TRecord> : IRecordEditorVM {
    public new TRecord Record { get; set; }
}
