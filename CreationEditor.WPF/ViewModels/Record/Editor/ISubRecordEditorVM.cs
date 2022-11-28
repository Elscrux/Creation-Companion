namespace CreationEditor.WPF.ViewModels.Record;

public interface ISubRecordEditorVM<TRecord> : IRecordEditorVM {
    public new TRecord Record { get; set; }
}