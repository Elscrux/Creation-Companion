using CreationEditor.Services.Integrations;
using CreationEditor.Services.Integrations.BSA;
using ReleasePipeline.Models;
using ReleasePipeline.Models.Actions;
namespace ReleasePipeline.Services.Actions;

/// <summary>
/// Pipeline action that creates a BSA archive using a configured BSA archive creator integration.
/// The action references a configured integration by id and invokes its archive-creation method.
/// </summary>
public sealed class BSAArchiveAction(IIntegrationRegistry registry, IIntegrationProvider provider) : IPipelineAction<BSAArchiveActionConfig> {
    public Guid Id { get; } = new("b7c2e3a1-4d4a-4f7a-9a4c-1e6a9b200003");
    public string DisplayName => "Create BSA Archive";
    public string Description => "Creates a BSA archive using a configured BSA archive creator tool.";
    public BSAArchiveActionConfig Config { get; set; } = new();
    public Type ConfigType => typeof(BSAArchiveActionConfig);

    public async Task<PipelineActionResult> ExecuteAsync(PipelineContext context, CancellationToken token) {
        var config = Config;

        var integration = registry.Integrations.FirstOrDefault(i => i.ImplementationId == config.SelectedIntegrationId);
        if (integration is null) {
            return PipelineActionResult.Fail("No BSA archive creator integration is configured for this action.");
        }

        var implementation = provider.GetImplementation(integration.ImplementationId);
        if (implementation is not IBSAArchiveCreator bsaCreator) {
            return PipelineActionResult.Fail("The selected integration is not a BSA archive creator.");
        }

        var sourceFolder = context.ReleaseFolder;
        var targetArchivePath = Path.Combine(context.ReleaseFolder, $"{context.Mod.ModKey}.bsa");

        var result = await bsaCreator.CreateArchiveAsync(
            sourceFolder,
            targetArchivePath,
            integration.Config ?? implementation.CreateDefaultConfig(),
            token);

        return result.Succeeded
            ? PipelineActionResult.Pass(result.Message)
            : PipelineActionResult.Fail(result.Message);
    }
}
