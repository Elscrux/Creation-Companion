using System.Diagnostics;
using System.IO.Abstractions;
using CreationEditor;
using Serilog;
namespace LipGenerator.Services;

public sealed record LipFuzerArgs(string LipFuzerPath) : IFuzGeneratorArgs;

public sealed class LipFuzerWrapper(
    IFileSystem fileSystem,
    ILogger logger,
    LipFuzerArgs args) : IFuzGenerator {

    public string LipFuzerPath { get; } = ValidatePath(fileSystem, args.LipFuzerPath);

    private static string ValidatePath(IFileSystem fileSystem, string path) {
        if (string.IsNullOrEmpty(path)) throw new ArgumentException("LipFuzer path must be provided", nameof(path));
        if (!fileSystem.File.Exists(path)) throw new FileNotFoundException($"LIPFuzer.exe not found at {path}");

        return path;
    }

    public void GenerateFuz(string srcDir, string dstDir, string audioExt) {
        var process = new Process {
            StartInfo = {
                FileName = LipFuzerPath,
                Arguments = $"-s \"{srcDir}\" -d \"{dstDir}\" -a {audioExt} -l lip",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (process.ExitCode != 0 || output.Length > 0) {
            logger.Here().Information("LipFuzer exited with code {ExitCode}:\n{Output}\n{Error}", process.ExitCode, output, error);
        } else {
            logger.Here().Debug("Generated fuz files from {SrcDir} to {DstDir} with audio extension {AudioExt}", srcDir, dstDir, audioExt);
        }
    }
}
