using System;
using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public class SpellFilter : RecordFilter<ISpellGetter> {
    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return Enum.GetNames<SpellType>()
            .Select(spellType => new RecordFilterListing(
                spellType,
                record => record.Type.ToString() == spellType));
    }
}
