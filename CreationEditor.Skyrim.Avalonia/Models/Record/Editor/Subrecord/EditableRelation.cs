using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;

public sealed partial class EditableRelation : ReactiveObject {
    [Reactive] public partial FormKey TargetFormKey { get; set; }
    [Reactive] public partial CombatReaction Reaction { get; set; }

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
