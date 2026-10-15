using CreationEditor.Services.Environment;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Environment;

public sealed class LoadOrderSortingTests {
    private static SkyrimMod CreateMod(string name, ModType modType = ModType.Plugin, params ModKey[] masters) {
        var mod = new SkyrimMod(new ModKey(name, modType), SkyrimRelease.SkyrimSE);
        foreach (var master in masters) {
            mod.ModHeader.MasterReferences.Add(new MasterReference { Master = master });
        }

        return mod;
    }

    private static List<ModKey> GetListedOrder(IEditorEnvironment editorEnvironment) =>
        editorEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey).ToList();

    [Theory] [CreationEditorAutoData]
    public void Update_MasterPluginsAreLoadedBeforePlugins(IEditorEnvironment editorEnvironment) {
        var masterMod = CreateMod("SortedMasterMod", ModType.Master);
        var pluginA = CreateMod("SortedPluginA");
        var pluginB = CreateMod("SortedPluginB");

        // The master plugin is added last on purpose - sorting must move it to the front
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(pluginA, pluginB, masterMod)
            .ActiveMod.New("NewActiveMod")
            .Build());

        var listedOrder = GetListedOrder(editorEnvironment);
        var masterIndex = listedOrder.IndexOf(masterMod.ModKey);
        Assert.True(masterIndex < listedOrder.IndexOf(pluginA.ModKey));
        Assert.True(masterIndex < listedOrder.IndexOf(pluginB.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void Update_MastersAreLoadedBeforeTheirDependents(IEditorEnvironment editorEnvironment) {
        var master = CreateMod("SortedDependency");
        var dependent = CreateMod("SortedDependent", masters: master.ModKey);

        // The dependent is added before its master on purpose - sorting must move the master in front
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(dependent, master)
            .ActiveMod.New("NewActiveMod")
            .Build());

        var listedOrder = GetListedOrder(editorEnvironment);
        Assert.True(listedOrder.IndexOf(master.ModKey) < listedOrder.IndexOf(dependent.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void Update_TransitiveMastersAreLoadedBeforeTheirDependents(IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("SortedModA");
        var modB = CreateMod("SortedModB", masters: modA.ModKey);
        var modC = CreateMod("SortedModC", masters: modB.ModKey);

        // The mods are added in reverse dependency order on purpose
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(modC, modB, modA)
            .ActiveMod.New("NewActiveMod")
            .Build());

        var listedOrder = GetListedOrder(editorEnvironment);
        Assert.True(listedOrder.IndexOf(modA.ModKey) < listedOrder.IndexOf(modB.ModKey));
        Assert.True(listedOrder.IndexOf(modB.ModKey) < listedOrder.IndexOf(modC.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void Update_UnrelatedModsKeepTheirRelativeOrder(IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("SortedModA");
        var modB = CreateMod("SortedModB");
        var modC = CreateMod("SortedModC");

        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(modC, modA, modB)
            .ActiveMod.New("NewActiveMod")
            .Build());

        var listedOrder = GetListedOrder(editorEnvironment);
        Assert.True(listedOrder.IndexOf(modC.ModKey) < listedOrder.IndexOf(modA.ModKey));
        Assert.True(listedOrder.IndexOf(modA.ModKey) < listedOrder.IndexOf(modB.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void Update_SelfMaster_KeepsModInLoadOrder(IEditorEnvironment editorEnvironment) {
        var mod = CreateMod("SelfMasterMod");
        mod.ModHeader.MasterReferences.Add(new MasterReference { Master = mod.ModKey });

        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(mod)
            .ActiveMod.New("NewActiveMod")
            .Build());

        Assert.Contains(mod.ModKey, GetListedOrder(editorEnvironment));
    }

    [Theory] [CreationEditorAutoData]
    public void Update_CyclicMasterReferences_KeepsModsInLoadOrder(IEditorEnvironment editorEnvironment) {
        var modA = CreateMod("CyclicModA");
        var modB = CreateMod("CyclicModB", masters: modA.ModKey);
        modA.ModHeader.MasterReferences.Add(new MasterReference { Master = modB.ModKey });

        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(modA, modB)
            .ActiveMod.New("NewActiveMod")
            .Build());

        var listedOrder = GetListedOrder(editorEnvironment);
        Assert.Contains(modA.ModKey, listedOrder);
        Assert.Contains(modB.ModKey, listedOrder);
    }
}

public sealed class LoadOrderSorterTests {
    private static LoadOrderSortResult Sort(LoadOrderSorter sorter, IReadOnlyList<ModKey> modKeys, params (ModKey Mod, ModKey[] Masters)[] masterMap) {
        return sorter.Sort(modKeys, key => masterMap.Find(x => x.Mod == key).Masters ?? []);
    }

    [Theory] [CreationEditorAutoData]
    public void Sort_MasterPluginsComeBeforePlugins(LoadOrderSorter sorter) {
        var master = new ModKey("SortMaster", ModType.Master);
        var plugin = new ModKey("SortPlugin", ModType.Plugin);

        var result = Sort(sorter, [plugin, master]);

        Assert.Equal([master, plugin], result.Order);
        Assert.Empty(result.Issues);
    }

    [Theory] [CreationEditorAutoData]
    public void Sort_MastersComeBeforeTheirDependents(LoadOrderSorter sorter) {
        var master = new ModKey("SortMaster", ModType.Plugin);
        var dependent = new ModKey("SortDependent", ModType.Plugin);

        var result = Sort(sorter, [dependent, master], (dependent, [master]));

        Assert.Equal([master, dependent], result.Order);
        Assert.Empty(result.Issues);
    }

    [Theory] [CreationEditorAutoData]
    public void Sort_TransitiveMastersComeFirst(LoadOrderSorter sorter) {
        var modA = new ModKey("SortModA", ModType.Plugin);
        var modB = new ModKey("SortModB", ModType.Plugin);
        var modC = new ModKey("SortModC", ModType.Plugin);

        var result = Sort(sorter, [modC, modB, modA], (modB, [modA]), (modC, [modB]));

        Assert.Equal([modA, modB, modC], result.Order);
        Assert.Empty(result.Issues);
    }

    [Theory] [CreationEditorAutoData]
    public void Sort_UnrelatedModsKeepTheirRelativeOrder(LoadOrderSorter sorter) {
        var modA = new ModKey("SortModA", ModType.Plugin);
        var modB = new ModKey("SortModB", ModType.Plugin);
        var modC = new ModKey("SortModC", ModType.Plugin);

        var result = Sort(sorter, [modC, modA, modB]);

        Assert.Equal([modC, modA, modB], result.Order);
        Assert.Empty(result.Issues);
    }

    [Theory] [CreationEditorAutoData]
    public void Sort_MastersNotInLoadOrderAreIgnored(LoadOrderSorter sorter) {
        var mod = new ModKey("SortMod", ModType.Plugin);

        var result = Sort(sorter, [mod], (mod, [new ModKey("MissingMaster", ModType.Plugin)]));

        Assert.Equal([mod], result.Order);
        Assert.Empty(result.Issues);
    }

    [Theory] [CreationEditorAutoData]
    public void Sort_SelfMasterIsReported(LoadOrderSorter sorter) {
        var mod = new ModKey("SelfMasterMod", ModType.Plugin);

        var result = Sort(sorter, [mod], (mod, [mod]));

        var issue = Assert.Single(result.Issues);
        Assert.Equal(LoadOrderIssueType.SelfMaster, issue.Type);
        Assert.Equal([mod], issue.Mods);
        Assert.Equal([mod], result.Order);
    }

    [Theory] [CreationEditorAutoData]
    public void Sort_CyclicMasterReferencesAreReported(LoadOrderSorter sorter) {
        var modA = new ModKey("CyclicModA", ModType.Plugin);
        var modB = new ModKey("CyclicModB", ModType.Plugin);

        var result = Sort(sorter, [modA, modB], (modA, [modB]), (modB, [modA]));

        var issue = Assert.Single(result.Issues);
        Assert.Equal(LoadOrderIssueType.CyclicDependency, issue.Type);
        Assert.Contains(modA, issue.Mods);
        Assert.Contains(modB, issue.Mods);
        // All mods are still part of the load order
        Assert.Equal(2, result.Order.Count);
    }

    [Theory] [CreationEditorAutoData]
    public void Sort_CyclicMasterReferencesDoNotAffectOtherMods(LoadOrderSorter sorter) {
        var modA = new ModKey("CyclicModA", ModType.Plugin);
        var modB = new ModKey("CyclicModB", ModType.Plugin);
        var independent = new ModKey("IndependentMod", ModType.Plugin);

        var result = Sort(sorter, [modA, independent, modB], (modA, [modB]), (modB, [modA]));

        Assert.Equal([independent, modA, modB], result.Order);
        var issue = Assert.Single(result.Issues);
        Assert.Equal(LoadOrderIssueType.CyclicDependency, issue.Type);
    }
}
