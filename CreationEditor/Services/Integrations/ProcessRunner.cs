using System.Diagnostics;
namespace CreationEditor.Services.Integrations;

public sealed record ProcessRunnerResult(
    int ExitCode,
    string Output,
    string Error);

public static class ProcessRunner {
    public static async Task<ProcessRunnerResult> RunProcessAsync(
        string executablePath,
        string arguments,
        CancellationToken cancellationToken) {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo {
            FileName = executablePath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        process.Start();

        var outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var errorTask = process.StandardError.ReadToEndAsync(cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var output = await outputTask;
        var error = await errorTask;

        return new ProcessRunnerResult(
            process.ExitCode,
            output,
            error);
    }
}
