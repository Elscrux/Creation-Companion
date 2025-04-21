namespace CreationEditor.Avalonia.ViewModels.Dialog;

public interface ISaveDialogVM {
    IObservable<bool>? CanSave { get; }
    IObservable<string>? SaveButtonContent { get; }

    Task<bool> Save();
}
