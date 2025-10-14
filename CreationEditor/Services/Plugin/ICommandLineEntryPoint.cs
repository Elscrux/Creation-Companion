namespace CreationEditor.Services.Plugin;

public interface ICommandLineEntryPoint {
    IReadOnlySet<string> Verbs { get; }

    Task<int> Run(string[] args);
}

public interface IDataSourceArguments {
    string ActiveDataSourcePath { get; }
    IEnumerable<string> AdditionalDataSourcePaths { get; }
    bool IncludeDataDirectoryDataSource { get; }
}
