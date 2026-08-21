using CommandLine;
namespace CreationEditor.Skyrim.Avalonia.Options;

/// <summary>
/// Global command-line options
/// </summary>
public sealed record GlobalOptions {
    [Option('g', "game-directory", Required = false, HelpText = "Override the game directory path")]
    public string? GameDirectory { get; init; }
}
