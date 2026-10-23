using System.Reactive.Linq;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Test;

public sealed class RecordReferenceControllerTests {
    /// <summary>
    /// Waits until the reference service finished loading all references (records and assets),
    /// so that record mutations are reliably picked up by the reference tracking.
    /// </summary>
    private static async Task WaitForReferenceLoading(IReferenceService referenceService) {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var subscription = referenceService.IsLoading
            .Where(loading => !loading)
            .Subscribe(_ => completion.TrySetResult());

        if (!referenceService.IsLoadingCurrently) {
            completion.TrySetResult();
        }

        await completion.Task.WaitAsync(TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Applies an environment change and waits for the triggered reference reload to finish.
    /// Subscribes before the change so the loading transition cannot be missed.
    /// </summary>
    private static async Task ApplyAndWaitForReload(IReferenceService referenceService, Action action) {
        var sawLoading = false;
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var subscription = referenceService.IsLoading.Subscribe(loading => {
            if (loading) {
                sawLoading = true;
            } else if (sawLoading) {
                completion.TrySetResult();
            }
        });

        action();

        await completion.Task.WaitAsync(TimeSpan.FromSeconds(500));
    }

    [Theory] [CreationEditorAutoData]
    public async Task Test_ReferenceUpdate_AfterRecordChange(
        IRecordController recordController,
        IReferenceService referenceService) {
        await WaitForReferenceLoading(referenceService);

        // Create record with no references
        var armorAddon = recordController.CreateRecord<ArmorAddon, IArmorAddonGetter>();
        var armor = recordController.CreateRecord<Armor, IArmorGetter>();

        // Check the other record has no references
        Assert.Equal(0, referenceService.GetRecordReferences(armorAddon).Count());

        // Add reference to the record
        recordController.RegisterUpdate(armor, () => armor.Armature.Add(armorAddon.FormKey));

        // Check the other record has references now
        Assert.Equal(1, referenceService.GetRecordReferences(armorAddon).Count());

        // Add reference to the record
        recordController.DeleteRecord(armor);

        // Check the other record has no references now
        Assert.Equal(0, referenceService.GetRecordReferences(armorAddon).Count());
    }

    [Theory] [CreationEditorAutoData]
    public async Task Test_ReferenceUpdate_AfterRecordChange_Multiple(
        IRecordController recordController,
        IReferenceService referenceService) {
        await WaitForReferenceLoading(referenceService);

        // Create record with no references
        var armorAddon = recordController.CreateRecord<ArmorAddon, IArmorAddonGetter>();
        const int count = 10;
        var armors = Enumerable.Range(0, count).Select(_ => recordController.CreateRecord<Armor, IArmorGetter>()).ToList();

        // Check the other record has no references
        Assert.Equal(0, referenceService.GetRecordReferences(armorAddon).Count());

        // Add reference to the record
        foreach (var armor in armors) {
            recordController.RegisterUpdate(armor, () => armor.Armature.Add(armorAddon.FormKey));
        }

        // Check the other record has references now
        Assert.Equal(count, referenceService.GetRecordReferences(armorAddon).Count());
    }

    [Theory] [CreationEditorAutoData]
    public async Task Test_ReferenceUpdate_AfterLoadOrderChange_ActiveModSwap(
        SkyrimMod masterMod,
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        IReferenceService referenceService) {
        // Create record with no references
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        var armorAddon = new ArmorAddon(masterMod);
        masterMod.ArmorAddons.Add(armorAddon);

        // Build the initial environment with the master mod
        await ApplyAndWaitForReload(referenceService,
            () => editorEnvironment.Update(updater => updater
                .LoadOrder.SetMutableMods(masterMod)
                .ActiveMod.New("NewMod")
                .Build()));

        // Add a reference to the record in the current active mod
        var armorOverride = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);
        recordController.RegisterUpdate(armorOverride, () => armorOverride.Armature.Add(armorAddon.FormKey));

        // Check the other record has references now
        Assert.Equal(1, referenceService.GetRecordReferences(armorAddon).Count());

        // Now swap the active mod
        await ApplyAndWaitForReload(referenceService, () => editorEnvironment.SetActive("TotallyNewMod"));

        // Check the reference is gone
        Assert.Equal(0, referenceService.GetRecordReferences(armorAddon).Count());
    }

    [Theory] [CreationEditorAutoData]
    public async Task Test_ReferenceUpdate_AfterLoadOrderChange_MutableModAdded(
        SkyrimMod additionalMutableMod,
        IEditorEnvironment editorEnvironment,
        IReferenceService referenceService) {
        await WaitForReferenceLoading(referenceService);

        // Create record with no references
        var armor = new Armor(additionalMutableMod);
        additionalMutableMod.Armors.Add(armor);
        var armorAddon = new ArmorAddon(additionalMutableMod);
        additionalMutableMod.ArmorAddons.Add(armorAddon);
        armor.Armature.Add(armorAddon.FormKey);

        // Check the other record has references now
        Assert.Equal(0, referenceService.GetRecordReferences(armorAddon).Count());

        // Load the mutable mod - this will end up before the active mod in the load order
        await ApplyAndWaitForReload(referenceService,
            () => editorEnvironment.Update(updater => updater.LoadOrder.AddMutableMods(additionalMutableMod).Build()));

        // Check that there is a reference now
        Assert.Equal(1, referenceService.GetRecordReferences(armorAddon).Count());

        // Unload the mutable mod
        await ApplyAndWaitForReload(referenceService, () => editorEnvironment.RemoveMutableMod(additionalMutableMod));

        // Check the reference is gone
        Assert.Equal(0, referenceService.GetRecordReferences(armorAddon).Count());
    }

    [Theory] [CreationEditorAutoData]
    public async Task Test_ReferenceUpdate_AfterLoadOrderChange_ActiveModHasPriorityOverMutable(
        SkyrimMod masterMod,
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        IReferenceService referenceService) {
        // Create two record with references in the master mod
        var armor = new Armor(masterMod);
        var armor2 = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        masterMod.Armors.Add(armor2);
        var armorAddon = new ArmorAddon(masterMod);
        masterMod.ArmorAddons.Add(armorAddon);
        armor.Armature.Add(armorAddon.FormKey);
        armor2.Armature.Add(armorAddon.FormKey);

        // Build the initial environment with the master mod
        await ApplyAndWaitForReload(referenceService,
            () => editorEnvironment.Update(updater => updater
                .LoadOrder.SetMutableMods(masterMod)
                .ActiveMod.New("NewMod")
                .Build()));

        // Check there are two references in the master mod
        Assert.Equal(2, referenceService.GetRecordReferences(armorAddon).Count());

        // Remove the references from the active mod
        var overrideArmor = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);
        recordController.RegisterUpdate(overrideArmor, () => overrideArmor.Armature.Remove(armorAddon.FormKey));
        var overrideArmor2 = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor2);
        recordController.RegisterUpdate(overrideArmor2, () => overrideArmor2.Armature.Remove(armorAddon.FormKey));

        // Check the override record has no references now, because the active mod is removing them
        Assert.Equal(0, referenceService.GetRecordReferences(armorAddon).Count());

        // Load the mutable mod - this will end up before the active mod in the load order
        var additionalMutableMod = new SkyrimMod(ModKey.FromName("additionalMutableMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        await ApplyAndWaitForReload(referenceService,
            () => editorEnvironment.Update(updater => updater.LoadOrder.AddMutableMods(additionalMutableMod).Build()));

        // Remove the reference of one record from the mutable mod
        var overrideArmorMutable = recordController.GetOrAddOverride<Armor, IArmorGetter>(armor, additionalMutableMod);
        recordController.RegisterUpdate(overrideArmorMutable, additionalMutableMod, () => overrideArmorMutable.Armature.Remove(armorAddon.FormKey));

        // Check there are still no references
        Assert.Equal(0, referenceService.GetRecordReferences(armorAddon).Count());

        // Load different active mod
        await ApplyAndWaitForReload(referenceService, () => editorEnvironment.SetActive("NewActiveMod"));

        await ApplyAndWaitForReload(referenceService,
            () => editorEnvironment.Update(updater => updater.LoadOrder.AddMutableMods(additionalMutableMod).Build()));

        // Check if the mutable mod references are now visible
        Assert.Equal(1, referenceService.GetRecordReferences(armorAddon).Count());
    }
}
