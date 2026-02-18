using System.Collections;
using CreationEditor.Services.Environment;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace AnalyzerPlugin.Services;

public class TopicEnricher(ILinkCacheProvider linkCacheProvider) {
    private bool HasEnrichTargets((string Name, object Value)[] metaData) {
        return metaData
            .Select(x => x.Value)
            .Any(IsEnrichTarget);
    }

    private bool IsEnrichTarget(object obj) {
        switch (obj) {
            case IReadOnlyCollection<IFormLinkGetter> coll:
                return coll.Count > 0;
            case IDictionary dict:
                if (dict.Count == 0) return false;

                foreach (DictionaryEntry dictItem in dict) {
                    if (IsEnrichTarget(dictItem.Key)) return true;
                    if (dictItem.Value is not null && IsEnrichTarget(dictItem.Value)) return true;

                    return false;
                }
                return false;
            case IFormLinkGetter:
            case IEnumerable<IFormLinkGetter>:
                return true;
            case IEnumerable<object> enumerable:
                return enumerable.Any(IsEnrichTarget);
            default:
                return false;
        }
    }

    public Topic Enrich(Topic topic) => topic with {
        FormattedTopic = topic.FormattedTopic.Transform(new object(), Selector),
        MetaData = Enrich(topic.MetaData)
    };

    private (string Name, object Value)[] Enrich((string Name, object Value)[] metaData) {
        if (!HasEnrichTargets(metaData)) return metaData;

        return metaData
            .Select(item => (item.Name, EnrichItem(item.Value)))
            .ToArray();
    }

    private object? Selector(object? _, object? item) => item switch {
        IFormLinkGetter link => LinkResolver(link),
        _ => item
    };

    private object EnrichItem(object item) => item switch {
        IFormLinkGetter link => LinkResolver(link),
        IDictionary dictionary => dictionary.Keys.Cast<object>()
            .ToDictionary(key => key switch {
                    IFormLinkGetter keyLink => LinkResolver(keyLink),
                    _ => EnrichItem(key)
                },
                key => {
                    var value = dictionary[key];
                    return value switch {
                        IFormLinkGetter valueLink => LinkResolver(valueLink),
                        null => null,
                        _ => EnrichItem(value)
                    };
                }),
        IEnumerable<IFormLinkGetter> enumerable => enumerable
            .Select(LinkResolver)
            .ToArray(),
        IEnumerable<object> enumerable => enumerable
            .Select(x => x switch {
                IFormLinkGetter link => LinkResolver(link),
                _ => EnrichItem(x)
            })
            .ToArray(),
        _ => item
    };

    private object LinkResolver(IFormLinkGetter link) {
        if (linkCacheProvider.LinkCache.TryResolveIdentifier(link.FormKey, link.Type, out var editorId)) {
            return new MajorRecordIdentifier {
                FormKey = link.FormKey,
                EditorID = editorId
            };
        }

        return link;
    }
}
