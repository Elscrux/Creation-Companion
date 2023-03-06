using System.Collections.Generic;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public class ClassFilter : SimpleRecordFilter<IClassGetter> {
    public ClassFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Training", record => record.Teaches.HasValue && record.Teaches.Value != 0),
    }) {}
}
