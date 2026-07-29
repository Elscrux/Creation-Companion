using System.Collections.Concurrent;
using CreationEditor.Services.Mutagen.Record;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace SearchPlugin.Models;

public abstract class TextSearcher<TMod, TModGetter, TMajor, TMajorGetter>(IRecordController recordController) : ITextSearcher<TMod, TModGetter>
    where TMajor : class, TMajorGetter, IMajorRecordQueryable
    where TMajorGetter : class, IMajorRecordQueryableGetter
    where TModGetter : class, IModGetter
    where TMod : class, TModGetter, IMod {
    private static readonly ConcurrentDictionary<FormKey, Lock> Locks = [];
    public abstract string SearcherName { get; }

    public IEnumerable<TextReference> GetTextReference(IMajorRecordGetterEnumerable mod, string reference, StringComparison comparison) {
        foreach (var record in mod.EnumerateMajorRecords<TMajorGetter>()) {
            var references = GetText(record)
                .WhereNotNull()
                .Where(text => text.Contains(reference, comparison))
                .Select(text => new TextDiff(text, text))
                .ToArray();

            if (references.Length == 0) continue;

            foreach (var textDiff in references) {
                yield return new TextReference(this, record, textDiff);
            }
        }
    }

    public void ReplaceTextReference(
        IMajorRecordQueryableGetter record,
        ILinkCache linkCache,
        IMod mod,
        string oldText,
        string newText,
        StringComparison comparison) {
        if (linkCache is not ILinkCache<TMod, TModGetter> typedLinkCache) return;
        if (mod is not TMod typedMod) return;

        ReplaceTextReference(record, typedLinkCache, typedMod, oldText, newText, comparison);
    }

    public void ReplaceTextReference(
        IMajorRecordQueryableGetter record,
        ILinkCache<TMod, TModGetter> linkCache,
        TMod mod,
        string oldText,
        string newText,
        StringComparison comparison) {
        if (record is not TMajorGetter) return;
        if (record is not IMajorRecordGetter majorRecord) return;

        var formKey = majorRecord.FormKey;
        var recordLock = Locks.GetOrAdd(formKey);

        lock (recordLock) {
            if (recordController.GetOrAddOverride(majorRecord, mod) is not TMajor overrideRecord) return;

            if (record is IDialogTopicGetter topic) {
                // TEMP FIX - Make sure response count is sustained as Mutagen currently just counts
                // the number of responses in the current mod and not the overwritten mods.
                foreach (var response in topic.Responses) {
                    recordController.GetOrAddOverride(response, mod);
                }
            }

            ReplaceText(overrideRecord, oldText, newText, comparison);
        }

        Locks.TryRemove(formKey, out _);
    }

    protected abstract IEnumerable<string?> GetText(TMajorGetter record);
    protected abstract void ReplaceText(TMajor record, string oldText, string newText, StringComparison comparison);

    protected virtual bool Equals(TextSearcher<TMod, TModGetter, TMajor, TMajorGetter> other) {
        return SearcherName == other.SearcherName;
    }

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;

        return Equals((TextSearcher<TMod, TModGetter, TMajor, TMajorGetter>) obj);
    }

    public override int GetHashCode() {
        return SearcherName.GetHashCode();
    }

    public override string ToString() {
        return SearcherName;
    }
}
