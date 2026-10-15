using System.Reactive.Subjects;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Analyzer;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Analyzer;
using CreationEditor.Avalonia.Views.Analyzer;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.ViewModels.Analyzer;

public partial class AnalyzerResultVM(
    IReferenceService referenceService,
    ILinkCacheProvider linkCacheProvider,
    IContextMenuProvider contextMenuProvider,
    AnalyzerResultInfo info,
    IFormattedTopicFormatter topicFormatter) : ViewModel {
    private readonly Subject<AnalyzerResultVM> _configureRequested = new();

    public AnalyzerResultInfo Info { get; } = info;

    public AnalyzerResult Result => Info.Result;
    public string? ResultEditorId => Info.ResultEditorId;
    public string? RecordDisplayName => Info.RecordDisplayName;
    public string? ParentDisplayName => Info.ParentDisplayName;
    public IMajorRecordIdentifierGetter? ParentIdentifier => Info.ParentIdentifier;
    public IReadOnlyList<FormattedTopicSegment> MessageSegments { get; } = topicFormatter.Format(info.Result.Topic.FormattedTopic);

    public IObservable<AnalyzerResultVM> ConfigureRequested => _configureRequested;

    public string GetIdentifier() => Info.GetIdentifier();

    public IEnumerable<Control> GetContextMenuItems() {
        if (Result.Record is null || !linkCacheProvider.LinkCache.TryResolve(Result.Record, out var record)) return [];

        using var disposable = referenceService.GetReferencedRecord(record, out var referencedRecord);
        return contextMenuProvider.GetMenuItems(new SelectedListContext([new RecordContext(Info.Result.ModKey, referencedRecord)], []));
    }
}
