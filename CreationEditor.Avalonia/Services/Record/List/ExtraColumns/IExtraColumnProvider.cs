using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public interface IExtraColumnProvider {
    /// <summary>
    /// Extra columns per type
    /// </summary>
    public IReadOnlyDictionary<Type, IExtraColumns> ExtraColumnsCache { get; }
}
