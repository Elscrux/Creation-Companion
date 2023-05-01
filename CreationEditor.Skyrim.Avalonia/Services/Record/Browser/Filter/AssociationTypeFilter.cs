using System.Collections.Generic;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class AssociationTypeFilter : SimpleRecordFilter<IAssociationTypeGetter> {
    public AssociationTypeFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Family", record => record.IsFamily is true),
        new("Non Family", record => record.IsFamily is not true),
    }) {}
}
