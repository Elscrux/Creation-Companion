using CreationEditor.Avalonia.ViewModels;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// UI wrapper around a single pre-release check, tracking its run status so the view can
/// render a per-check box with a pass/fail icon.
/// </summary>
public sealed partial class CheckItemVM : ViewModel {
    public IReleaseCheck Check { get; }

    public string Name => Check.Name;
    public CheckSeverity Severity => Check.Severity;

    [Reactive] public partial CheckStatus Status { get; set; }
    [Reactive] public partial string? Message { get; set; }

    public CheckItemVM(IReleaseCheck check) {
        Check = check;
        Status = CheckStatus.Pending;
    }

    public void ApplyResult(CheckResult result) {
        Status = result.Status;
        Message = result.Message;
    }
}
