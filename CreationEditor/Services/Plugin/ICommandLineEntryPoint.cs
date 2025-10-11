namespace CreationEditor.Services.Plugin;

public interface ICommandLineEntryPoint {
    IReadOnlySet<string> Verbs { get; }

    Task<int> Run(string[] args);
}
