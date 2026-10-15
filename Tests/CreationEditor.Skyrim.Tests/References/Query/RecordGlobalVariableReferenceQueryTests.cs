using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Cache;
using CreationEditor.Services.Mutagen.References.Query;
using CreationEditor.Services.Mutagen.Type;
using CreationEditor.Skyrim.Tests.AutoData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
namespace CreationEditor.Skyrim.Tests.References.Query;

public sealed class RecordGlobalVariableReferenceQueryTests {
    [Theory] [CreationEditorAutoData]
    public void FillCache_BookText_MapsGlobalVariables(
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        IMutagenCommonAspectsProvider commonAspectsProvider,
        IRecordController recordController) {
        var book = recordController.CreateRecord<Book, IBookGetter>();
        recordController.RegisterUpdate(book, () => book.BookText.String = "Read <Global=MyGlobal> and <Global.Hour12=TimeGlobal>");

        var cache = DictionaryReferenceCache<string, IFormLinkIdentifier>.CreateNew();
        new RecordGlobalVariableReferenceQuery(commonAspectsProvider).FillCache(editorEnvironment.ActiveMod, cache);

        Assert.True(cache.Cache.TryGetValue("MyGlobal", out var myGlobalRefs));
        Assert.Contains(myGlobalRefs, r => r.FormKey == book.FormKey);
        Assert.True(cache.Cache.TryGetValue("TimeGlobal", out var timeGlobalRefs));
        Assert.Contains(timeGlobalRefs, r => r.FormKey == book.FormKey);
    }

    [Theory] [CreationEditorAutoData]
    public void FillCache_MessageTitleAndDescription_MapsGlobalVariables(
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        IMutagenCommonAspectsProvider commonAspectsProvider,
        IRecordController recordController) {
        var message = recordController.CreateRecord<Message, IMessageGetter>();
        recordController.RegisterUpdate(message,
            () => {
                message.Description.String = "<Global=DescriptionGlobal>";
                message.Name = new TranslatedString(Language.English);
                message.Name.String = "<Global=TitleGlobal>";
            });

        var cache = DictionaryReferenceCache<string, IFormLinkIdentifier>.CreateNew();
        new RecordGlobalVariableReferenceQuery(commonAspectsProvider).FillCache(editorEnvironment.ActiveMod, cache);

        Assert.True(cache.Cache.TryGetValue("TitleGlobal", out var titleRefs));
        Assert.Contains(titleRefs, r => r.FormKey == message.FormKey);
        Assert.True(cache.Cache.TryGetValue("DescriptionGlobal", out var descriptionRefs));
        Assert.Contains(descriptionRefs, r => r.FormKey == message.FormKey);
    }

    [Theory] [CreationEditorAutoData]
    public void FillCache_TextWithoutGlobals_AddsNoEntries(
        IEditorEnvironment<ISkyrimMod, ISkyrimModGetter> editorEnvironment,
        IMutagenCommonAspectsProvider commonAspectsProvider,
        IRecordController recordController) {
        var book = recordController.CreateRecord<Book, IBookGetter>();
        recordController.RegisterUpdate(book, () => book.BookText.String = "No globals in here");

        var cache = DictionaryReferenceCache<string, IFormLinkIdentifier>.CreateNew();
        new RecordGlobalVariableReferenceQuery(commonAspectsProvider).FillCache(editorEnvironment.ActiveMod, cache);

        Assert.Empty(cache.Cache);
    }

    [Theory] [CreationEditorAutoData]
    public void ParseRecord_ReturnsGlobalNames(
        IMutagenCommonAspectsProvider commonAspectsProvider,
        IRecordController recordController) {
        var book = recordController.CreateRecord<Book, IBookGetter>();
        recordController.RegisterUpdate(book, () => book.BookText.String = "<Global=MyGlobal>");

        var globalNames = new RecordGlobalVariableReferenceQuery(commonAspectsProvider).ParseRecord(book).ToList();

        Assert.Contains("MyGlobal", globalNames);
    }

    [Theory] [CreationEditorAutoData]
    public void GlobalVariableRegex_MatchesSupportedProperties() {
        var matches = RecordGlobalVariableReferenceQuery.GlobalVariableRegex.Matches(
            "<Global=Plain> <Global.Hour12=H> <Global.MonthWord=M> <Global.TimeSpan=T> <Global.Unknown=X>");

        Assert.Equal(4, matches.Count);
        Assert.Equal("Plain", matches[0].Groups[1].Value);
        Assert.Equal("H", matches[1].Groups[1].Value);
        Assert.Equal("M", matches[2].Groups[1].Value);
        Assert.Equal("T", matches[3].Groups[1].Value);
    }
}
