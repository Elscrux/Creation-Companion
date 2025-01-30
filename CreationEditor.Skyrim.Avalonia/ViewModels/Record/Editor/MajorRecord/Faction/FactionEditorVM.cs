using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Skyrim.Avalonia.Services.Record.Editor;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using EditableFaction = CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord.EditableFaction;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Faction;

public sealed class FactionEditorVM : ViewModel, IRecordEditorVM<Mutagen.Bethesda.Skyrim.Faction, IFactionGetter> {
    IRecordEditorCore IRecordEditorVM.Core => Core;
    public IRecordEditorCore<EditableFaction, Mutagen.Bethesda.Skyrim.Faction, IFactionGetter> Core { get; }

    public Func<IPlacedGetter, bool> ChestFilter { get; }
    public RelationEditorVM? RelationEditorVM { get; set; }
    public RankEditorVM? RankEditorVM { get; set; }

    public IConditionCopyPasteController ConditionsCopyPasteController { get; }

    public FactionEditorVM(
        Mutagen.Bethesda.Skyrim.Faction faction,
        Func<Mutagen.Bethesda.Skyrim.Faction, EditableRecordConverter<EditableFaction, Mutagen.Bethesda.Skyrim.Faction, IFactionGetter>, IRecordEditorCore<EditableFaction, Mutagen.Bethesda.Skyrim.Faction, IFactionGetter>> coreFactory,
        IConditionCopyPasteController conditionsCopyPasteController) {
        ConditionsCopyPasteController = conditionsCopyPasteController;

        var converter = new EditableRecordConverter<EditableFaction, Mutagen.Bethesda.Skyrim.Faction, IFactionGetter>(
            f => new EditableFaction(f),
            f => f.DeepCopy());
        Core = coreFactory(faction, converter).DisposeWith(this);

        RelationEditorVM?.Dispose();
        RelationEditorVM = new RelationEditorVM(this);
        RelationEditorVM.DisposeWith(this);
        RankEditorVM?.Dispose();
        RankEditorVM = new RankEditorVM(this);
        RankEditorVM.DisposeWith(this);

        ChestFilter = placed => {
            if (placed is not IPlacedObjectGetter placedObject) return false;

            var placeableObjectGetter = placedObject.Base.TryResolve(Core.LinkCacheProvider.LinkCache);
            return placeableObjectGetter is IContainerGetter;
        };
    }
}
