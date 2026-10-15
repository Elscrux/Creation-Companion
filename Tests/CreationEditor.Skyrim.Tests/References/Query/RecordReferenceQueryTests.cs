using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Query;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Tests.References.Query;

public sealed class RecordReferenceQueryTests {
    private static (SkyrimMod Mod, Armor Armor, ArmorAddon ArmorAddon) CreateModWithReference() {
        var mod = new SkyrimMod(ModKey.FromName("TestMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        var armorAddon = new ArmorAddon(mod);
        mod.ArmorAddons.Add(armorAddon);
        var armor = new Armor(mod);
        mod.Armors.Add(armor);
        armor.Armature.Add(armorAddon.FormKey);
        return (mod, armor, armorAddon);
    }

    [Fact]
    public void FillCache_AddsAllRecordFormKeys() {
        var (mod, armor, armorAddon) = CreateModWithReference();
        var cache = RecordReferenceCache.CreateNew();

        new RecordReferenceQuery().FillCache(mod, cache);

        Assert.Contains(armor.FormKey, cache.FormKeys);
        Assert.Contains(armorAddon.FormKey, cache.FormKeys);
        Assert.Equal(2, cache.FormKeys.Count);
    }

    [Fact]
    public void FillCache_MapsReferencesToReferencedRecords() {
        var (mod, armor, armorAddon) = CreateModWithReference();
        var cache = RecordReferenceCache.CreateNew();

        new RecordReferenceQuery().FillCache(mod, cache);

        var references = cache.Cache[armorAddon.FormKey];
        Assert.Contains(references, r => r.FormKey == armor.FormKey);
    }

    [Fact]
    public void FillCache_RecordWithoutReferences_HasNoCacheEntry() {
        var mod = new SkyrimMod(ModKey.FromName("TestMod", ModType.Plugin), SkyrimRelease.SkyrimSE);
        var armor = new Armor(mod);
        mod.Armors.Add(armor);
        var cache = RecordReferenceCache.CreateNew();

        new RecordReferenceQuery().FillCache(mod, cache);

        Assert.Empty(cache.Cache);
    }

    [Fact]
    public void FillCache_ExcludesNullLinks() {
        var (mod, _, _) = CreateModWithReference();
        var cache = RecordReferenceCache.CreateNew();

        new RecordReferenceQuery().FillCache(mod, cache);

        Assert.DoesNotContain(FormKey.Null, cache.Cache.Keys);
        Assert.All(cache.Cache.Values, references => Assert.All(references, r => Assert.False(r.FormKey.IsNull)));
    }

    [Fact]
    public void ParseRecord_ReturnsFormLinksOfRecord() {
        var (_, armor, armorAddon) = CreateModWithReference();

        var links = RecordReferenceQuery.ParseRecord(armor).ToList();

        Assert.Contains(links, l => l.FormKey == armorAddon.FormKey);
    }
}
