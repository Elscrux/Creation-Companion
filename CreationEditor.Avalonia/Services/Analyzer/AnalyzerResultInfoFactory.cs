using CreationEditor.Avalonia.Models.Analyzer;
using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Analyzer;

public sealed class AnalyzerResultInfoFactory : IAnalyzerResultInfoFactory {
    public AnalyzerResultInfo Create(AnalyzerResult result, ILinkCache linkCache) {
        string? resultEditorId = null;
        string? recordDisplayName;
        string? parentDisplayName = null;
        IMajorRecordIdentifierGetter? parentIdentifier = null;

        if (result.Record is not null) {
            resultEditorId = linkCache.TryResolveIdentifier(result.Record, out var resolvedEditorId) ? resolvedEditorId : null;

            recordDisplayName = resultEditorId
             ?? Describe(result.Record, linkCache)
             ?? result.Record.FormKey.ToString();

            if (linkCache.TryResolveSimpleContext(result.Record, out var parentContext)
             && parentContext.Parent?.Record is IMajorRecordGetter parentRecord) {
                parentIdentifier = new MajorRecordIdentifier {
                    FormKey = parentRecord.FormKey,
                    EditorID = parentRecord.EditorID,
                };
                parentDisplayName = DisplayName(parentRecord);
            }
        } else {
            recordDisplayName = "Unknown Record";
        }

        return new AnalyzerResultInfo {
            Result = result,
            ResultEditorId = resultEditorId,
            RecordDisplayName = recordDisplayName,
            ParentDisplayName = parentDisplayName,
            ParentIdentifier = parentIdentifier,
        };
    }

    private static string? Describe(IFormLinkIdentifier record, ILinkCache linkCache) {
        if (linkCache.TryResolve(record, out var resolvedRecord)) {
            return DisplayName(resolvedRecord);
        }

        return null;
    }

    private static string DisplayName(IMajorRecordGetter record) => record.EditorID ?? record.FormKey.ToString();
}
