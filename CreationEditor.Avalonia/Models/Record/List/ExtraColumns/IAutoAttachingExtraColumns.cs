namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public interface IAutoAttachingExtraColumns : IUntypedExtraColumns {
    bool CanAttachTo(Type type);
}
