using CreationEditor.Services.Environment;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.Environment;

public sealed class EditorEnvironmentTests {
    [Theory] [CreationEditorAutoData]
    public void Update_FiresEnvironmentEvents(IEditorEnvironment editorEnvironment) {
        var loadOrders = new List<List<ModKey>>();
        using var loadOrderSubscription = editorEnvironment.LoadOrderChanged.Subscribe(loadOrders.Add);
        var activeMods = new List<ModKey>();
        using var activeModSubscription = editorEnvironment.ActiveModChanged.Subscribe(activeMods.Add);
        var linkCaches = new List<ILinkCache>();
        using var linkCacheSubscription = editorEnvironment.LinkCacheChanged.Subscribe(linkCaches.Add);

        var masterMod = new SkyrimMod(ModKey.FromName("MasterMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(masterMod)
            .ActiveMod.New("OtherActiveMod")
            .Build());

        Assert.NotEmpty(loadOrders);
        Assert.NotEmpty(activeMods);
        Assert.NotEmpty(linkCaches);
        Assert.Equal(editorEnvironment.ActiveMod.ModKey, activeMods[^1]);
        Assert.Equal(editorEnvironment.LinkCache, linkCaches[^1]);
    }

    [Theory] [CreationEditorAutoData]
    public void LoadOrder_ActiveModIsLastInListedOrder(
        IEditorEnvironment editorEnvironment) {
        var masterMod = new SkyrimMod(ModKey.FromName("MasterMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(masterMod)
            .ActiveMod.New("NewActiveMod")
            .Build());

        var listedOrder = editorEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey).ToList();
        Assert.Equal(editorEnvironment.ActiveMod.ModKey, listedOrder[^1]);
        Assert.Contains(masterMod.ModKey, listedOrder);
    }

    [Theory] [CreationEditorAutoData]
    public void AddMutableMods_InsertsBeforeActiveMod(
        IEditorEnvironment editorEnvironment) {
        var masterMod = new SkyrimMod(ModKey.FromName("MasterMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(masterMod)
            .ActiveMod.New("NewActiveMod")
            .Build());

        var middleMod = new SkyrimMod(ModKey.FromName("MiddleMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        editorEnvironment.Update(updater => updater.LoadOrder.AddMutableMods(middleMod).Build());

        var listedOrder = editorEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey).ToList();
        var activeIndex = listedOrder.IndexOf(editorEnvironment.ActiveMod.ModKey);
        var middleIndex = listedOrder.IndexOf(middleMod.ModKey);
        var masterIndex = listedOrder.IndexOf(masterMod.ModKey);

        Assert.True(middleIndex > masterIndex);
        Assert.True(middleIndex < activeIndex);
        Assert.Contains(middleMod, editorEnvironment.MutableMods);
    }

    [Theory] [CreationEditorAutoData]
    public void RemoveMutableMod_RemovesModFromLoadOrder(
        IEditorEnvironment editorEnvironment) {
        var masterMod = new SkyrimMod(ModKey.FromName("MasterMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(masterMod)
            .ActiveMod.New("NewActiveMod")
            .Build());

        editorEnvironment.RemoveMutableMod(masterMod);

        Assert.DoesNotContain(masterMod.ModKey, editorEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey));
        Assert.DoesNotContain(masterMod, editorEnvironment.MutableMods);
    }

    [Theory] [CreationEditorAutoData]
    public void SetActive_New_SwitchesToNewEmptyActiveMod(
        IEditorEnvironment editorEnvironment) {
        var previousActive = editorEnvironment.ActiveMod;

        editorEnvironment.SetActive("OtherActiveMod");

        Assert.Equal(new ModKey("OtherActiveMod", ModType.Plugin), editorEnvironment.ActiveMod.ModKey);
        Assert.NotEqual(previousActive.ModKey, editorEnvironment.ActiveMod.ModKey);
        Assert.Empty(editorEnvironment.ActiveMod.EnumerateMajorRecords());
    }

    [Theory] [CreationEditorAutoData]
    public void SetActive_Existing_MissingModFile_Throws(
        IEditorEnvironment editorEnvironment) {
        Assert.Throws<FileNotFoundException>(() => editorEnvironment.SetActive(new ModKey("MissingMod", ModType.Plugin)));
    }

    [Theory] [CreationEditorAutoData]
    public void AddNewMutableMod_CreatesEmptyModBeforeActiveMod(
        IEditorEnvironment editorEnvironment) {
        editorEnvironment.AddNewMutableMod(new ModKey("AddedMod", ModType.Plugin));

        var addedMod = editorEnvironment.GetMutableMod(new ModKey("AddedMod", ModType.Plugin));
        Assert.NotNull(addedMod);
        Assert.Empty(addedMod.EnumerateMajorRecords());

        var listedOrder = editorEnvironment.LinkCache.ListedOrder.Select(x => x.ModKey).ToList();
        Assert.True(listedOrder.IndexOf(addedMod.ModKey) < listedOrder.IndexOf(editorEnvironment.ActiveMod.ModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void AddNewMutableMod_ThrowsIfReadOnlyModAlreadyExists(
        IEditorEnvironment editorEnvironment) {
        // A read-only mod that is already present in the load order cannot be added as a mutable mod
        var readOnlyModKey = editorEnvironment.LinkCache.ListedOrder[0].ModKey;

        Assert.Throws<InvalidOperationException>(() => editorEnvironment.AddNewMutableMod(readOnlyModKey));
    }

    [Theory] [CreationEditorAutoData]
    public void MutableMods_ReflectsLoadOrderMutableMods(
        IEditorEnvironment editorEnvironment) {
        var masterMod = new SkyrimMod(ModKey.FromName("MasterMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(masterMod)
            .ActiveMod.New("NewActiveMod")
            .Build());

        var mutableModKeys = editorEnvironment.MutableMods.Select(x => x.ModKey).ToList();

        // Mutable mods are all mods in the load order that are mutable, including the active mod
        var expected = editorEnvironment.LinkCache.ListedOrder.OfType<ISkyrimMod>().Select(x => x.ModKey).ToList();
        Assert.Equal(expected, mutableModKeys);
        Assert.Contains(masterMod.ModKey, mutableModKeys);
        Assert.Contains(editorEnvironment.ActiveMod.ModKey, mutableModKeys);
    }
}
