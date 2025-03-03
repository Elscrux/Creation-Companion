using System.IO.Abstractions;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Test;

public sealed class RecordReferenceControllerTests {
    [Theory, CreationEditorAutoData]
    public void Test_ReferenceUpdate_AfterRecordChange(
        IRecordController recordController,
        RecordReferenceController recordReferenceController) {
        // Create record with no references
        var armorAddon = recordController.CreateRecord<ArmorAddon, IArmorAddonGetter>();
        var armor = recordController.CreateRecord<Armor, IArmorGetter>();

        // Check the other record has no references
        Assert.Equal(0, recordReferenceController.GetReferences(armorAddon.FormKey).Count());

        // Add reference to the record
        recordController.RegisterUpdate(armor, () => armor.Armature.Add(armorAddon));

        // Check the other record has references now
        Assert.Equal(1, recordReferenceController.GetReferences(armorAddon.FormKey).Count());

        // Add reference to the record
        recordController.DeleteRecord(armor);

        // Check the other record has no references now
        Assert.Equal(0, recordReferenceController.GetReferences(armorAddon.FormKey).Count());
    }

    [Theory, CreationEditorAutoData]
    public void Test_ReferenceUpdate_AfterRecordChange_Multiple(
        IRecordController recordController,
        RecordReferenceController recordReferenceController) {
        // Create record with no references
        var armorAddon = recordController.CreateRecord<ArmorAddon, IArmorAddonGetter>();
        const int count = 10;
        var armors = Enumerable.Range(0, count).Select(_ => recordController.CreateRecord<Armor, IArmorGetter>()).ToList();

        // Check the other record has no references
        Assert.Equal(0, recordReferenceController.GetReferences(armorAddon.FormKey).Count());

        // Add reference to the record
        foreach (var armor in armors) {
            recordController.RegisterUpdate(armor, () => armor.Armature.Add(armorAddon));
        }

        // Check the other record has references now
        Assert.Equal(count, recordReferenceController.GetReferences(armorAddon.FormKey).Count());
    }

    [Theory, CreationEditorAutoData]
    public async Task Test_ReferenceUpdate_AfterLoadOrderChange_ActiveModSwap(
        SkyrimMod masterMod,
        IFileSystem fileSystem,
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        RecordReferenceController recordReferenceController,
        IDataDirectoryProvider dataDirectoryProvider) {
        // Create record with no references
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        var armorAddon = new ArmorAddon(masterMod);
        masterMod.ArmorAddons.Add(armorAddon);

        // Build the initial environment with the master mod
        var modPath = fileSystem.Path.Combine(dataDirectoryProvider.Path, masterMod.ModKey.FileName);
        masterMod.WriteToBinary(modPath, new BinaryWriteParameters { FileSystem = fileSystem });
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetImmutableMods([masterMod.ModKey])
            .ActiveMod.New("NewMod")
            .Build());

        // Add a reference to the record in the current active mod
        var armorOverride = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);
        recordController.RegisterUpdate(armorOverride, () => armorOverride.Armature.Add(armorAddon));

        // Check the other record has references now
        Assert.Equal(1, recordReferenceController.GetReferences(armorAddon.FormKey).Count());

        // Now swap the active mod
        editorEnvironment.SetActive("TotallyNewMod");

        await Task.Delay(250); // wait for the new editor environment to propagate a little

        // Check the reference is gone
        Assert.Equal(0, recordReferenceController.GetReferences(armorAddon.FormKey).Count());
    }

    [Theory, CreationEditorAutoData]
    public async Task Test_ReferenceUpdate_AfterLoadOrderChange_MutableModAdded(
        SkyrimMod additionalMutableMod,
        IEditorEnvironment editorEnvironment,
        RecordReferenceController recordReferenceController) {
        // Create record with no references
        var armor = new Armor(additionalMutableMod);
        additionalMutableMod.Armors.Add(armor);
        var armorAddon = new ArmorAddon(additionalMutableMod);
        additionalMutableMod.ArmorAddons.Add(armorAddon);
        armor.Armature.Add(armorAddon);

        // Check the other record has references now
        Assert.Equal(0, recordReferenceController.GetReferences(armorAddon.FormKey).Count());

        // Load the mutable mod - this will end up before the active mod in the load order
        editorEnvironment.Update(updater => updater.LoadOrder.AddMutableMod(additionalMutableMod).Build());
        await Task.Delay(2050); // wait for the new editor environment to propagate a little

        // Check that there is a reference now
        Assert.Equal(1, recordReferenceController.GetReferences(armorAddon.FormKey).Count());

        // Unload the mutable mod
        editorEnvironment.RemoveMutableMod(additionalMutableMod);
        await Task.Delay(250); // wait for the new editor environment to propagate a little

        // Check the reference is gone
        Assert.Equal(0, recordReferenceController.GetReferences(armorAddon.FormKey).Count());
    }

    [Theory, CreationEditorAutoData]
    public async Task Test_ReferenceUpdate_AfterLoadOrderChange_ActiveModHasPriorityOverMutable(
        SkyrimMod masterMod,
        IFileSystem fileSystem,
        IEditorEnvironment editorEnvironment,
        IRecordController recordController,
        RecordReferenceController recordReferenceController,
        IDataDirectoryProvider dataDirectoryProvider) {
        // Create two record with references in the master mod
        var armor = new Armor(masterMod);
        var armor2 = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        masterMod.Armors.Add(armor2);
        var armorAddon = new ArmorAddon(masterMod);
        masterMod.ArmorAddons.Add(armorAddon);
        armor.Armature.Add(armorAddon);
        armor2.Armature.Add(armorAddon);

        // Build the initial environment with the master mod
        var modPath = fileSystem.Path.Combine(dataDirectoryProvider.Path, masterMod.ModKey.FileName);
        masterMod.WriteToBinary(modPath, new BinaryWriteParameters { FileSystem = fileSystem });
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetImmutableMods([masterMod.ModKey])
            .ActiveMod.New("NewMod")
            .Build());

        await Task.Delay(250); // wait for the new editor environment to propagate a little

        // Check there are two references in the master mod
        Assert.Equal(2, recordReferenceController.GetReferences(armorAddon.FormKey).Count());

        // Remove the references from the active mod
        var overrideArmor = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor);
        recordController.RegisterUpdate(overrideArmor, () => overrideArmor.Armature.Remove(armorAddon));
        var overrideArmor2 = recordController.GetOrAddOverride<IArmor, IArmorGetter>(armor2);
        recordController.RegisterUpdate(overrideArmor2, () => overrideArmor2.Armature.Remove(armorAddon));

        // Check the override record has no references now, because the active mod is removing them
        Assert.Equal(0, recordReferenceController.GetReferences(armorAddon.FormKey).Count());

        // Load the mutable mod - this will end up before the active mod in the load order
        var additionalMutableMod = new SkyrimMod(ModKey.FromName("additionalMutableMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        editorEnvironment.Update(updater => updater.LoadOrder.AddMutableMod(additionalMutableMod).Build());
        await Task.Delay(250); // wait for the new editor environment to propagate a little

        // Remove the reference of one record from the mutable mod
        var overrideArmorMutable = recordController.GetOrAddOverride<Armor, IArmorGetter>(armor, additionalMutableMod);
        recordController.RegisterUpdate(overrideArmorMutable, additionalMutableMod, () => overrideArmorMutable.Armature.Remove(armorAddon));

        // Check there are still no references
        Assert.Equal(0, recordReferenceController.GetReferences(armorAddon.FormKey).Count());

        // Load different active mod
        editorEnvironment.SetActive("NewActiveMod");

        await Task.Delay(250); // wait for the new editor environment to propagate a little
        editorEnvironment.Update(updater => updater.LoadOrder.AddMutableMod(additionalMutableMod).Build());
        await Task.Delay(250); // wait for the new editor environment to propagate a little

        // Check if the mutable mod references are now visible
        Assert.Equal(1, recordReferenceController.GetReferences(armorAddon.FormKey).Count());
    }
}
