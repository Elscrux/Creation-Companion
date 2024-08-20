using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;

public sealed class EditableRelation : ReactiveObject {
    [Reactive] public FormKey TargetFormKey { get; set; }
    [Reactive] public CombatReaction Reaction { get; set; }

    public Relation ToRelation() {
        return new Relation { Reaction = Reaction, Target = new FormLink<IRelatableGetter>(TargetFormKey) };
    }

    public static EditableRelation? Factory(object o) {
        return o switch {
            EditableRelation relation => relation,
            IFormLinkIdentifier identifier =>
                identifier.Type.InheritsFrom(typeof(IRelatableGetter)) ?
                    new EditableRelation {
                        TargetFormKey = identifier.FormKey,
                        Reaction = CombatReaction.Neutral,
                    } : null,
            _ => null,
        };
    }
}
