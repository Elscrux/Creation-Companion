using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace SearchPlugin.Models;

public interface ITextSearcher : ITextSearcherDefinition {
    IEnumerable<TextReference> GetTextReference(IMajorRecordGetterEnumerable mod, string reference, StringComparison comparison);
    void ReplaceTextReference(IMajorRecordQueryableGetter record, ILinkCache linkCache, IMod mod, string oldText, string newText, StringComparison comparison);
}

public interface ITextSearcher<TMod, TModGetter> : ITextSearcher
    where TModGetter : class, IModGetter
    where TMod : class, TModGetter, IMod {
    void ReplaceTextReference(IMajorRecordQueryableGetter record, ILinkCache<TMod, TModGetter> linkCache, TMod mod, string oldText, string newText, StringComparison comparison);
}
