namespace ReleasePipeline.Models.Actions;

/// <summary>Configuration for <see cref="BSAArchiveAction"/>.</summary>
public sealed class BSAArchiveActionConfig {
    /// <summary>Id of the configured BSA archive creator integration to use.</summary>
    public Guid SelectedIntegrationId { get; set; }
}
