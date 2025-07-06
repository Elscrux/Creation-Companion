namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public interface IConditionalExtraColumns : IUntypedExtraColumns {
    bool CanAttachTo(Type type);
}
