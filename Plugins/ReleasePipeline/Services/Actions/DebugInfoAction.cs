using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
using Serilog;
namespace ReleasePipeline.Services.Actions;

/// <summary>Configuration for <see cref="DebugInfoAction"/>.</summary>
public sealed class DebugInfoConfig {
    /// <summary>Optional extra message appended to the debug output.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Simulated work duration in seconds, so the pipeline visibly progresses.</summary>
    public int DelaySeconds { get; set; } = 5;
}

/// <summary>
/// Debug action that reports the pipeline context. Useful to validate the runner end to end.
/// </summary>
public sealed class DebugInfoAction(ILogger logger) : IPipelineAction<DebugInfoConfig> {
    public Guid Id { get; } = new("b7c2e3a1-4d4a-4f7a-9a4c-1e6a9b200001");
    public string DisplayName => "Debug Info";
    public string Description => "Logs the current pipeline context for validating the framework.";
    public DebugInfoConfig Config { get; set; } = new();
    public Type ConfigType => typeof(DebugInfoConfig);

    public async Task<PipelineActionResult> ExecuteAsync(PipelineContext context, CancellationToken token) {
        var config = Config;
        logger.Information(
            "Release Pipeline Debug Info. Mod: {Mod}, ReleaseFolder: {Folder}, Message: {Message}",
            context.Mod.ModKey,
            context.ReleaseFolder,
            config.Message);

        // Simulate work so the pipeline run is visibly progressing.
        if (config.DelaySeconds > 0) {
            await Task.Delay(TimeSpan.FromSeconds(config.DelaySeconds), token);
        }

        return PipelineActionResult.Pass(
            $"Mod '{context.Mod.ModKey}' will be released to '{context.ReleaseFolder}'." +
            (string.IsNullOrWhiteSpace(config.Message) ? string.Empty : $" ({config.Message})"));
    }
}
