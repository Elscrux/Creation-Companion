using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;

public sealed class EditableRelation : ReactiveObject {
    [Reactive] public FormKey TargetFormKey { get; set; }
    [Reactive] public CombatReaction Reaction { get; set; }

    public Relation ToRelation() {
        return new Relation { Reaction = Reaction, Target = new FormLink<IRelatableGetter>(TargetFormKey) };
    }
}
