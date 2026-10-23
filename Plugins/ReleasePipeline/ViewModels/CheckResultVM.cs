using CreationEditor.Avalonia.Models.Analyzer;
using CreationEditor.Avalonia.Services.Analyzer;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Analyzer;
using CreationEditor.Services.Environment;
using ReactiveUI.SourceGenerators;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.ViewModels;

/// <summary>
/// UI wrapper around a single pre-release check, tracking its run status so the view can
/// render a per-check box with a pass/fail icon.
/// </summary>
public sealed partial class CheckResultVM : ViewModel {
    private readonly Func<AnalyzerResultInfo, AnalyzerResultVM> _analyzerResultVMFactory;
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IAnalyzerResultInfoFactory _analyzerResultInfoFactory;

    public IReleaseCheck Check { get; }

    public string Name => Check.Name;

    [Reactive] public partial CheckStatus Status { get; set; }
    [Reactive] public partial IReadOnlyList<AnalyzerResultVM> Results { get; set; } = [];

    public CheckResultVM(
        Func<AnalyzerResultInfo, AnalyzerResultVM> analyzerResultVMFactory,
        IReleaseCheck check,
        ILinkCacheProvider linkCacheProvider,
        IAnalyzerResultInfoFactory analyzerResultInfoFactory) {
        _analyzerResultVMFactory = analyzerResultVMFactory;
        _linkCacheProvider = linkCacheProvider;
        _analyzerResultInfoFactory = analyzerResultInfoFactory;
        Check = check;
        Status = CheckStatus.Pending;
    }

    public void ApplyResult(CheckResult result) {
        Status = result.Status;
        Results = result.Results
            .Select(r => _analyzerResultInfoFactory.Create(r, _linkCacheProvider.LinkCache))
            .Select(r => _analyzerResultVMFactory(r))
            .ToArray();
    }
}
