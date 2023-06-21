using System.Collections.Generic;
using System.Linq;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Avalonia.Services.Record.Browser.Filter;

public sealed class RaceFilter : SimpleRecordFilter<IRaceGetter> {
    public RaceFilter() : base(new List<SimpleRecordFilterEntry> {
        new("Child", record => (record.Flags & Race.Flag.Child) != 0),
        new("Playable", record => (record.Flags & Race.Flag.Playable) != 0),
        new("NPC", record => record.Keywords is not null && record.Keywords.Contains(Mutagen.Bethesda.FormKeys.SkyrimSE.Skyrim.Keyword.ActorTypeNPC)),
        new("Creature", record => record.Keywords is not null && record.Keywords.Contains(Mutagen.Bethesda.FormKeys.SkyrimSE.Skyrim.Keyword.ActorTypeCreature)),
    }) {}
}
