using CreationEditor.Services.Mutagen.Mod.Save;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Mutagen.Save;

public sealed class DeletedNewRecordRemoveStepTests {
    private static SkyrimMod CreateMod(string name) => new(new ModKey(name, ModType.Plugin), SkyrimRelease.SkyrimSE);

    [Fact]
    public void DeletedModCreatedRecord_IsRemoved() {
        var mod = CreateMod("TestMod");
        var armor = new Armor(mod) { EditorID = "NewArmor", IsDeleted = true };
        mod.Armors.Add(armor);

        new DeletedNewRecordRemoveStep().Execute(null!, mod);

        Assert.Empty(mod.Armors);
    }

    [Fact]
    public void NonDeletedModCreatedRecord_IsKept() {
        var mod = CreateMod("TestMod");
        var armor = new Armor(mod) { EditorID = "NewArmor" };
        mod.Armors.Add(armor);

        new DeletedNewRecordRemoveStep().Execute(null!, mod);

        Assert.Single(mod.Armors);
    }

    [Fact]
    public void DeletedOverrideOfMasterRecord_IsKept() {
        var masterMod = CreateMod("Master");
        var masterArmor = new Armor(masterMod) { EditorID = "MasterArmor" };
        masterMod.Armors.Add(masterArmor);

        var overrideMod = CreateMod("Override");
        var overrideArmor = overrideMod.Armors.GetOrAddAsOverride(masterArmor);
        overrideArmor.IsDeleted = true;

        new DeletedNewRecordRemoveStep().Execute(null!, overrideMod);

        // Overrides belong to the master mod and this edit is meant to remove it,
        // so this step doesn't remove it as it's a change with a purpose
        Assert.Single(overrideMod.Armors);
    }
}