namespace ReleasePipeline.ViewModels;

/// <summary>
/// Ordered phases of the release flow, driven by the main wizard VM.
/// </summary>
public enum PipelineWizardStep {
    /// <summary>Run and review the fixed pre-release check suite.</summary>
    Checks,
    /// <summary>Pick an existing pipeline (platform) or create a new one.</summary>
    SelectPipeline,
    /// <summary>Edit the selected pipeline's actions and run it.</summary>
    EditPipeline,
}
