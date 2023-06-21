using System;
using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class RelationshipFilter : RecordFilter<IRelationshipGetter> {
    private readonly IEditorEnvironment _editorEnvironment;

    public RelationshipFilter(
        IEditorEnvironment editorEnvironment) {
        _editorEnvironment = editorEnvironment;
    }

    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return _editorEnvironment.LinkCache.PriorityOrder.WinningOverrides<IAssociationTypeGetter>()
            .Where(associationType => associationType.EditorID is not null)
            .Select(associationType => new RecordFilterListing(associationType.EditorID!, record => record is IRelationshipGetter relationship && relationship.AssociationType.FormKey == associationType.FormKey));
    }
}
