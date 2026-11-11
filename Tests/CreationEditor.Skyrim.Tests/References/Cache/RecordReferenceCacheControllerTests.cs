using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.References.Cache;

public sealed class RecordReferenceCacheControllerTests {
    [Theory] [CreationEditorAutoData]
    public void GetLinks_ResolvesReferenceAndReturnsItsFormLinks(
        SkyrimMod masterMod,
        IEditorEnvironment editorEnvironment) {
        // Create a record that references another record
        var armorAddon = new ArmorAddon(masterMod);
        masterMod.ArmorAddons.Add(armorAddon);
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);
        armor.Armature.Add(armorAddon.FormKey);

        // Load the mod into the environment so the link cache can resolve the record
        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(masterMod)
            .ActiveMod.New("NewMod")
            .Build());

        var controller = new RecordReferenceCacheController(editorEnvironment);

        var links = controller.GetLinks([], armor).ToList();

        Assert.Contains(links, l => l.FormKey == armorAddon.FormKey);
    }

    [Theory] [CreationEditorAutoData]
    public void GetLinks_RecordWithoutLinks_ReturnsEmpty(
        SkyrimMod masterMod,
        IEditorEnvironment editorEnvironment) {
        // Create a record with no references
        var armor = new Armor(masterMod);
        masterMod.Armors.Add(armor);

        editorEnvironment.Update(updater => updater
            .LoadOrder.SetMutableMods(masterMod)
            .ActiveMod.New("NewMod")
            .Build());

        var controller = new RecordReferenceCacheController(editorEnvironment);

        Assert.Empty(controller.GetLinks([], armor));
    }

    [Theory] [CreationEditorAutoData]
    public void AddLink_RegistersReferenceRecordAndAddsReferenceForEveryLink(
        SkyrimMod mod,
        IEditorEnvironment editorEnvironment) {
        var controller = new RecordReferenceCacheController(editorEnvironment);
        var cache = RecordReferenceCache.CreateNew();
        var armor = new Armor(mod);
        var armorAddon = new ArmorAddon(mod);

        controller.AddLink(cache, armor, [armorAddon]);

        // The reference record itself is registered
        Assert.Contains(armor.FormKey, cache.FormKeys);
        // The reference is added for every link
        Assert.Contains(cache.Cache[armorAddon.FormKey], r => r.FormKey == armor.FormKey);
    }

    [Theory] [CreationEditorAutoData]
    public void RemoveLink_RemovesReferenceFromLinks(
        SkyrimMod mod,
        IEditorEnvironment editorEnvironment) {
        var controller = new RecordReferenceCacheController(editorEnvironment);
        var cache = RecordReferenceCache.CreateNew();
        var armor = new Armor(mod);
        var armorAddon = new ArmorAddon(mod);
        controller.AddLink(cache, armor, [armorAddon]);

        controller.RemoveLink(cache, armor, [armorAddon]);

        Assert.DoesNotContain(cache.Cache[armorAddon.FormKey], r => r.FormKey == armor.FormKey);
    }
}
