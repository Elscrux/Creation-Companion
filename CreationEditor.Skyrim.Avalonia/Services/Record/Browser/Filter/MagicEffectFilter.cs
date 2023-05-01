using System;
using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class MagicEffectFilter : RecordFilter<IMagicEffectGetter> {
    public override IEnumerable<RecordFilterListing> GetListings(Type type) {
        return Enum.GetNames<MagicEffectArchetype.TypeEnum>()
            .Select(archetype => new RecordFilterListing(archetype, record => record is IMagicEffectGetter magicEffect && magicEffect.Archetype.Type.ToString() == archetype));
    }
}
