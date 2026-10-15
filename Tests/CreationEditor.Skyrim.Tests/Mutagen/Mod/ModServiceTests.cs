using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Skyrim.Services.Mod;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Mutagen.Mod;

public sealed class ModServiceTests {
    private static SkyrimMod CreateMod(string name, params ModKey[] masters) {
        var mod = new SkyrimMod(ModKey.FromName(name, ModType.Plugin), SkyrimRelease.SkyrimSE);
        foreach (var master in masters) {
            mod.ModHeader.MasterReferences.Add(new MasterReference { Master = master });
        }

        return mod;
    }

    private static void LoadMods(IEditorEnvironment editorEnvironment, params SkyrimMod[] mods) {
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(mods)
            .ActiveMod.New("NewMod")
            .Build());
    }

    [Theory] [CreationEditorAutoData]
    public void HasMasterTransitive_DirectMaster(
        IModService modService,
        IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("MasterA");
        var modB = CreateMod("MasterB", modA.ModKey);
        LoadMods(editorEnvironment, modA, modB);

        Assert.True(modService.HasMasterTransitive(modB.ModKey, modA.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void HasMasterTransitive_TransitiveMaster(
        IModService modService,
        IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("MasterA");
        var modB = CreateMod("MasterB", modA.ModKey);
        var modC = CreateMod("MasterC", modB.ModKey);
        LoadMods(editorEnvironment, modA, modB, modC);

        Assert.True(modService.HasMasterTransitive(modC.ModKey, modA.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void HasMasterTransitive_UnrelatedMod_ReturnsFalse(
        IModService modService,
        IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("MasterA");
        var modB = CreateMod("MasterB");
        LoadMods(editorEnvironment, modA, modB);

        Assert.False(modService.HasMasterTransitive(modB.ModKey, modA.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void HasMasterTransitive_UnknownMod_ReturnsFalse(
        IModService modService,
        IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("MasterA");
        LoadMods(editorEnvironment, modA);

        Assert.False(modService.HasMasterTransitive(new ModKey("UnknownMod", ModType.Plugin), modA.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void IsModOrHasMasterTransitive_SameMod_ReturnsTrue(
        IModService modService,
        IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("MasterA");
        LoadMods(editorEnvironment, modA);

        Assert.True(modService.IsModOrHasMasterTransitive(modA.ModKey, modA.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void TryGetMastersTransitive_ReturnsAllTransitiveMasters(
        IModService modService,
        IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("MasterA");
        var modB = CreateMod("MasterB", modA.ModKey);
        var modC = CreateMod("MasterC", modB.ModKey);
        LoadMods(editorEnvironment, modA, modB, modC);

        var result = modService.TryGetMastersTransitive(modC.ModKey, out var masters);

        Assert.True(result);
        Assert.Equal([modA.ModKey, modB.ModKey], masters.ToList());
    }

    [Theory] [CreationEditorAutoData]
    public void TryGetMastersTransitive_UnknownMod_ReturnsFalse(
        IModService modService,
        IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("MasterA");
        LoadMods(editorEnvironment, modA);

        var result = modService.TryGetMastersTransitive(new ModKey("UnknownMod", ModType.Plugin), out var masters);

        Assert.False(result);
        Assert.Empty(masters);
    }

    [Theory] [CreationEditorAutoData]
    public void MastersCache_InvalidatedOnLinkCacheChange(
        IModService modService,
        IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("MasterA");
        var modB = CreateMod("MasterB", modA.ModKey);
        LoadMods(editorEnvironment, modA, modB);

        Assert.True(modService.HasMasterTransitive(modB.ModKey, modA.ModKey));

        // Remove the master reference and refresh the environment - the cached answer must be dropped
        modB.ModHeader.MasterReferences.Clear();
        editorEnvironment.Update(updater => updater.Build());

        Assert.False(modService.HasMasterTransitive(modB.ModKey, modA.ModKey));
    }
}

public sealed class ModInfoProviderMasterInfoTests {
    private static ModInfo CreateModInfo(string name, params ModKey[] masters) => new(
        ModKey.FromName(name, ModType.Plugin),
        null,
        null,
        false,
        44,
        masters);

    [Theory] [CreationEditorAutoData]
    public void GetMasterInfos_CompilesTransitiveMasters(SkyrimModInfoProvider modInfoProvider) {
        var modA = CreateModInfo("MasterA");
        var modB = CreateModInfo("MasterB", modA.ModKey);
        var modC = CreateModInfo("MasterC", modB.ModKey);

        var masterInfos = modInfoProvider.GetMasterInfos([modA, modB, modC]);

        Assert.True(masterInfos[modA.ModKey].Valid);
        Assert.Empty(masterInfos[modA.ModKey].Masters);
        Assert.True(masterInfos[modB.ModKey].Valid);
        Assert.Equal([modA.ModKey], masterInfos[modB.ModKey].Masters.ToList());
        Assert.True(masterInfos[modC.ModKey].Valid);
        Assert.Equal([modA.ModKey, modB.ModKey], masterInfos[modC.ModKey].Masters.ToList());
    }

    [Theory] [CreationEditorAutoData]
    public void GetMasterInfos_MissingMaster_InvalidatesMod(SkyrimModInfoProvider modInfoProvider) {
        var modA = CreateModInfo("MasterA");
        var modC = CreateModInfo("MasterC", new ModKey("MissingMaster", ModType.Plugin));

        var masterInfos = modInfoProvider.GetMasterInfos([modA, modC]);

        Assert.False(masterInfos[modC.ModKey].Valid);
        Assert.Empty(masterInfos[modC.ModKey].Masters);
    }

    [Theory] [CreationEditorAutoData]
    public void GetMasterInfos_InvalidMaster_InvalidatesDependents(SkyrimModInfoProvider modInfoProvider) {
        var modX = CreateModInfo("MasterX", new ModKey("MissingMaster", ModType.Plugin));
        var modC = CreateModInfo("MasterC", modX.ModKey);

        var masterInfos = modInfoProvider.GetMasterInfos([modX, modC]);

        Assert.False(masterInfos[modX.ModKey].Valid);
        Assert.False(masterInfos[modC.ModKey].Valid);
    }

    [Theory] [CreationEditorAutoData]
    public void GetMasterInfos_MastersOrderedByLoadOrderIndex(SkyrimModInfoProvider modInfoProvider) {
        var modA = CreateModInfo("MasterA");
        var modB = CreateModInfo("MasterB");
        var modC = CreateModInfo("MasterC", modB.ModKey, modA.ModKey);

        var masterInfos = modInfoProvider.GetMasterInfos([modA, modB, modC]);

        // Masters are ordered by their position in the load order, not by declaration order
        Assert.Equal([modA.ModKey, modB.ModKey], masterInfos[modC.ModKey].Masters.ToList());
    }
}
