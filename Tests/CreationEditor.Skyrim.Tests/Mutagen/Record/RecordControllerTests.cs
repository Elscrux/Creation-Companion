using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Mutagen.Record;

using RecordModPair = (IMajorRecordGetter Record, IModGetter Mod);

public sealed class RecordControllerTests {
    private static void LoadMasterMod(IEditorEnvironment editorEnvironment, SkyrimMod masterMod) {
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(masterMod)
            .ActiveMod.New("NewMod")
            .Build());
    }

    [Theory] [CreationEditorAutoData]
    public void CreateRecord_AddsRecordToActiveMod(
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var created = new List<RecordModPair>();
        using var createdSubscription = recordController.RecordCreated.Subscribe(created.Add);
        var activeCreated = new List<IMajorRecordGetter>();
        using var activeSubscription = recordController.RecordInActiveModCreated.Subscribe(activeCreated.Add);

        var armor = recordController.CreateRecord<Armor, IArmorGetter>();

        Assert.Equal(editorEnvironment.ActiveMod.ModKey, armor.FormKey.ModKey);
        Assert.NotNull(((SkyrimMod) editorEnvironment.ActiveMod).Armors.FirstOrDefault(x => x.FormKey == armor.FormKey));
        var pair = Assert.Single(created);
        Assert.Equal(armor, pair.Record);
        Assert.Equal(editorEnvironment.ActiveMod, pair.Mod);
        Assert.Equal(armor, Assert.Single(activeCreated));
    }

    [Theory] [CreationEditorAutoData]
    public void CreateRecord_AssignsUniqueFormKeys(IRecordController recordController) {
        var armor1 = recordController.CreateRecord<Armor, IArmorGetter>();
        var armor2 = recordController.CreateRecord<Armor, IArmorGetter>();

        Assert.NotEqual(armor1.FormKey, armor2.FormKey);
    }

    [Theory] [CreationEditorAutoData]
    public void CreateRecord_InSpecificMod_DoesNotFireActiveModCreated(
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var activeCreated = new List<IMajorRecordGetter>();
        using var activeSubscription = recordController.RecordInActiveModCreated.Subscribe(activeCreated.Add);
        var otherMod = new SkyrimMod(ModKey.FromName("OtherMod", ModType.Plugin), SkyrimRelease.SkyrimSE);

        var armor = recordController.CreateRecord<Armor, IArmorGetter>(otherMod);

        Assert.Equal(otherMod.ModKey, armor.FormKey.ModKey);
        Assert.Empty(activeCreated);
    }

    [Theory] [CreationEditorAutoData]
    public void GetOrAddOverride_CreatesOverrideInActiveMod(
        SkyrimMod masterMod,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        LoadMasterMod(editorEnvironment, masterMod);

        var changed = new List<RecordModPair>();
        using var changedSubscription = recordController.RecordChanged.Subscribe(changed.Add);

        var overrideArmor = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);

        // Overrides keep the FormKey of the record they override, but live in the overriding mod
        Assert.Equal(armor.FormKey, overrideArmor.FormKey);
        Assert.Contains(overrideArmor, ((SkyrimMod) editorEnvironment.ActiveMod).Armors);
        Assert.Equal(overrideArmor, Assert.Single(changed).Record);
    }

    [Theory] [CreationEditorAutoData]
    public void GetOrAddOverride_IsIdempotent_AndDoesNotFireAgain(
        SkyrimMod masterMod,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        LoadMasterMod(editorEnvironment, masterMod);

        var changed = new List<RecordModPair>();
        using var changedSubscription = recordController.RecordChanged.Subscribe(changed.Add);

        var override1 = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);
        var override2 = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);

        Assert.Same(override1, override2);
        Assert.Single(changed);
    }

    [Theory] [CreationEditorAutoData]
    public void GetOrAddOverride_OfRecordInTargetMod_DoesNotFireRecordChanged(
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = recordController.CreateRecord<Armor, IArmorGetter>();

        var changed = new List<RecordModPair>();
        using var changedSubscription = recordController.RecordChanged.Subscribe(changed.Add);

        var result = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);

        Assert.Same(armor, result);
        Assert.Empty(changed);
    }

    [Theory] [CreationEditorAutoData]
    public void RegisterUpdate_FiresRecordChangedWithUpdatedRecord(
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = recordController.CreateRecord<Armor, IArmorGetter>();

        var changed = new List<RecordModPair>();
        using var changedSubscription = recordController.RecordChanged.Subscribe(changed.Add);

        recordController.RegisterUpdate(armor, () => armor.EditorID = "UpdatedArmor");

        var pair = Assert.Single(changed);
        Assert.Equal("UpdatedArmor", ((IArmorGetter) pair.Record).EditorID);
        Assert.Equal(editorEnvironment.ActiveMod, pair.Mod);
    }

    [Theory] [CreationEditorAutoData]
    public void RegisterUpdate_RunsSubscribersBeforeAndAfterUpdate(
        IRecordController recordController) {
        var armor = recordController.CreateRecord<Armor, IArmorGetter>();
        var seenEditorIds = new List<string?>();

        using var diffSubscription = recordController.RecordChangedDiff.Subscribe(pair => {
            seenEditorIds.Add(((IArmorGetter) pair.Record).EditorID);
            return _ => seenEditorIds.Add(((IArmorGetter) pair.Record).EditorID);
        });

        recordController.RegisterUpdate(armor, () => armor.EditorID = "UpdatedArmor");

        Assert.Equal([null, "UpdatedArmor"], seenEditorIds);
    }

    [Theory] [CreationEditorAutoData]
    public void WinningRecordChanged_OnlyFiresForWinningOverride(
        SkyrimMod masterMod,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        LoadMasterMod(editorEnvironment, masterMod);

        // Create an override in a mutable mod that sits before the active mod in the load order
        var middleMod = new SkyrimMod(ModKey.FromName("MiddleMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        editorEnvironment.Update(updater => updater.LoadOrder.AddMutableMods(middleMod).Build());
        var middleOverride = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor, middleMod);

        // The active mod override wins over the middle mod override
        var activeOverride = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);

        var winning = new List<RecordModPair>();
        using var winningSubscription = recordController.WinningRecordChanged.Subscribe(winning.Add);

        recordController.RegisterUpdate(middleOverride, middleMod, () => middleOverride.EditorID = "Middle");
        Assert.Empty(winning);

        recordController.RegisterUpdate(activeOverride, () => activeOverride.EditorID = "Active");
        Assert.Single(winning);
    }

    [Theory] [CreationEditorAutoData]
    public void DeleteRecord_CreatesDeletedOverride(
        SkyrimMod masterMod,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        LoadMasterMod(editorEnvironment, masterMod);

        var deleted = new List<RecordModPair>();
        using var deletedSubscription = recordController.RecordDeleted.Subscribe(deleted.Add);

        recordController.DeleteRecord(armor);

        var pair = Assert.Single(deleted);
        Assert.Equal(armor.FormKey, pair.Record.FormKey);
        Assert.Contains(pair.Record, ((SkyrimMod) editorEnvironment.ActiveMod).Armors);
        Assert.True(pair.Record.IsDeleted);
    }

    [Theory] [CreationEditorAutoData]
    public void DuplicateRecord_CreatesNewRecordInTargetMod(
        SkyrimMod masterMod,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = new Armor(masterMod) { EditorID = "MasterArmor" };
        masterMod.Armors.Add(armor);
        LoadMasterMod(editorEnvironment, masterMod);

        var created = new List<RecordModPair>();
        using var createdSubscription = recordController.RecordCreated.Subscribe(created.Add);

        var duplicate = recordController.DuplicateRecord<Armor, IArmorGetter>(armor);

        Assert.NotEqual(armor.FormKey, duplicate.FormKey);
        Assert.Equal(editorEnvironment.ActiveMod.ModKey, duplicate.FormKey.ModKey);
        Assert.Equal("MasterArmor", duplicate.EditorID);
        Assert.Equal(duplicate, Assert.Single(created).Record);
    }

    [Theory] [CreationEditorAutoData]
    public void MarkForDeletion_WithoutReferences_DeletesRecord(
        SkyrimMod masterMod,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        LoadMasterMod(editorEnvironment, masterMod);

        var deleted = new List<RecordModPair>();
        using var deletedSubscription = recordController.RecordDeleted.Subscribe(deleted.Add);

        var result = recordController.MarkForDeletion(armor, editorEnvironment.ActiveMod, () => []);

        Assert.True(result);
        var pair = Assert.Single(deleted);
        Assert.True(pair.Record.IsDeleted);
    }

    [Theory] [CreationEditorAutoData]
    public void MarkForDeletion_WithReferences_MarksEditorIdInstead(
        SkyrimMod masterMod,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = new Armor(masterMod) { EditorID = "MasterArmor" };
        masterMod.Armors.Add(armor);
        LoadMasterMod(editorEnvironment, masterMod);

        var deleted = new List<RecordModPair>();
        using var deletedSubscription = recordController.RecordDeleted.Subscribe(deleted.Add);

        var result = recordController.MarkForDeletion(
            armor,
            editorEnvironment.ActiveMod,
            () => [new FormLinkInformation(armor.FormKey, typeof(IArmorGetter))]);

        Assert.True(result);
        Assert.Empty(deleted);
        var overrideArmor = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);
        Assert.StartsWith("xDELETE", overrideArmor.EditorID);
    }

    [Theory] [CreationEditorAutoData]
    public void MarkForDeletion_ForceDelete_DeletesDespiteReferences(
        SkyrimMod masterMod,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        LoadMasterMod(editorEnvironment, masterMod);

        var deleted = new List<RecordModPair>();
        using var deletedSubscription = recordController.RecordDeleted.Subscribe(deleted.Add);

        var result = recordController.MarkForDeletion(
            armor,
            editorEnvironment.ActiveMod,
            () => [new FormLinkInformation(armor.FormKey, typeof(IArmorGetter))],
            true);

        Assert.True(result);
        var pair = Assert.Single(deleted);
        Assert.True(pair.Record.IsDeleted);
    }

    [Theory] [CreationEditorAutoData]
    public void RemapReferences_PointsReferencingRecordsToReplacement(
        SkyrimMod masterMod,
        IRecordController recordController,
        IEditorEnvironment editorEnvironment) {
        var armorAddon = new ArmorAddon(masterMod);
        masterMod.ArmorAddons.Add(armorAddon);
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        var referencingArmor = new Armor(masterMod);
        masterMod.Armors.Add(referencingArmor);
        referencingArmor.Armature.Add(armor.FormKey);
        LoadMasterMod(editorEnvironment, masterMod);

        var replacement = recordController.CreateRecord<Armor, IArmorGetter>();

        recordController.RemapReferences(
            armor,
            [new FormLinkInformation(referencingArmor.FormKey, typeof(IArmorGetter))],
            replacement);

        var referencingOverride = recordController.GetOrAddOverride<IArmor, IArmorGetter>(referencingArmor);
        var armatureFormKeys = referencingOverride.Armature.Select(x => x.FormKey).ToList();
        Assert.DoesNotContain(armor.FormKey, armatureFormKeys);
        Assert.Contains(replacement.FormKey, armatureFormKeys);
    }
}
