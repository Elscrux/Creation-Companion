using System;
using System.Collections.Generic;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class QuestFilter : RecordFilter<IQuestGetter> {
    private const char QuestFilterSeparator = '\\';

    private readonly IEditorEnvironment _editorEnvironment;

    public QuestFilter(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides<IQuestGetter>()
            .GetRecursiveListings(quest => quest.Filter, QuestFilterSeparator);
    }
}
