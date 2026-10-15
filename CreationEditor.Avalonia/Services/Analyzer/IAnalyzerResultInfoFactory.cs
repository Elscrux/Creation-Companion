using CreationEditor.Avalonia.Models.Analyzer;
using Mutagen.Bethesda.Analyzers.Reporting.Handlers;
using Mutagen.Bethesda.Plugins.Cache;
namespace CreationEditor.Avalonia.Services.Analyzer;

/// <summary>
///     Factory for creating enriched analyzer result info objects.
///     Handles resolution of EditorIDs, parent records, and display names
///     using a link cache while the environment is loaded.
/// </summary>
public interface IAnalyzerResultInfoFactory {
    AnalyzerResultInfo Create(AnalyzerResult result, ILinkCache linkCache);
}
