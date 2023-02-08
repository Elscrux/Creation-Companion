using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;

public sealed class EditableRelation {
    [Reactive] public IFormLink<IRelatableGetter> Target { get; init; } = new FormLink<IRelatableGetter>();
    [Reactive] public CombatReaction Reaction { get; set; }

    public Relation ToRelation() {
        return new Relation { Reaction = Reaction, Target = Target };
    }
}
