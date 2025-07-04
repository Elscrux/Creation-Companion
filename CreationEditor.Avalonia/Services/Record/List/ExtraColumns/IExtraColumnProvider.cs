using CreationEditor.Avalonia.Models.Record.List.ExtraColumns;
namespace CreationEditor.Avalonia.Services.Record.List.ExtraColumns;

public interface IExtraColumnProvider {
    /// <summary>
    /// Extra columns per type
    /// </summary>
    Dictionary<Type, List<IExtraColumns>> ExtraColumnsCache { get; }

    /// <summary>
    /// Extra columns that are automatically attached if a function evaluates to true.
    /// </summary>
    IReadOnlyList<IConditionalExtraColumns> AutoAttachingExtraColumnsCache { get; }
}
