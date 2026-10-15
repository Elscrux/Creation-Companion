namespace CreationEditor.Services.Config;

public interface IConfigDefinition {
    IReadOnlyList<IConfigOption> Options { get; }
}
