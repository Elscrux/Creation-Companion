using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
namespace SearchPlugin.Models;

public interface ITextSearcher<TMod, TModGetter> : ITextSearcherDefinition
    where TModGetter : class, IModGetter
    where TMod : class, TModGetter, IMod {
    public IEnumerable<RecordReferences<TMod, TModGetter>> GetTextReference(IMajorRecordGetterEnumerable mod, string reference, StringComparison comparison);
    public void ReplaceTextReference(IMajorRecordQueryableGetter record, ILinkCache<TMod, TModGetter> linkCache, TMod mod, string oldText, string newText, StringComparison comparison);
}