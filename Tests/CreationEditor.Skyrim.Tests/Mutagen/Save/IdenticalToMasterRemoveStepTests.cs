using CreationEditor.Services.Mutagen.Mod.Save;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Mutagen.Save;

public sealed class IdenticalToMasterRemoveStepTests {
    private static SkyrimMod CreateMod(string name) => new(new ModKey(name, ModType.Plugin), SkyrimRelease.SkyrimSE);

    private static ILinkCache CreateLinkCache(params ISkyrimModGetter[] mods) => mods.ToImmutableLinkCache();

    [Fact]
    public void IdenticalOverride_IsRemoved() {
        var masterMod = CreateMod("Master");
        var masterArmor = new Armor(masterMod) { EditorID = "MasterArmor" };
        masterMod.Armors.Add(masterArmor);

        var overrideMod = CreateMod("Override");
        overrideMod.Armors.GetOrAddAsOverride(masterArmor);

        // The mod being saved is the winning override, so it is last in the load order
        var linkCache = CreateLinkCache(masterMod, overrideMod);

        new IdenticalToMasterRemoveStep().Execute(linkCache, overrideMod);

        Assert.Empty(overrideMod.Armors);
    }

    [Fact]
    public void ModifiedOverride_IsKept() {
        var masterMod = CreateMod("Master");
        var masterArmor = new Armor(masterMod) { EditorID = "MasterArmor" };
        masterMod.Armors.Add(masterArmor);

        var overrideMod = CreateMod("Override");
        var overrideArmor = overrideMod.Armors.GetOrAddAsOverride(masterArmor);
        overrideArmor.EditorID = "ModifiedArmor";

        // The mod being saved is the winning override, so it is last in the load order
        var linkCache = CreateLinkCache(masterMod, overrideMod);

        new IdenticalToMasterRemoveStep().Execute(linkCache, overrideMod);

        Assert.Single(overrideMod.Armors);
    }

    [Fact]
    public void ModCreatedRecord_IsKept() {
        var mod = CreateMod("TestMod");
        var armor = new Armor(mod) { EditorID = "NewArmor" };
        mod.Armors.Add(armor);

        var linkCache = CreateLinkCache(mod);

        new IdenticalToMasterRemoveStep().Execute(linkCache, mod);

        Assert.Single(mod.Armors);
    }
}