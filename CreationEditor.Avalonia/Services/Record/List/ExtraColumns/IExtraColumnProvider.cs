using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public interface IExtraColumnProvider {
    public Dictionary<Type, IExtraColumns> ExtraColumnsCache { get; }
}
