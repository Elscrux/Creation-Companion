using System.Reflection;
using System.Text.RegularExpressions;
using CreationEditor.Avalonia.Converter;
using CreationEditor.Avalonia.Views.Analyzer;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Services.Analyzer;

public interface IFormattedTopicFormatter {
    IReadOnlyList<FormattedTopicSegment> Format(IFormattedTopicDefinition? formattedTopicDefinition);
}

public sealed partial class FormattedTopicFormatter : IFormattedTopicFormatter {
    [GeneratedRegex(@"\{\d+\}")]
    private static partial Regex PlaceholderRegex { get; }

    public IReadOnlyList<FormattedTopicSegment> Format(IFormattedTopicDefinition? formattedTopicDefinition) {
        if (formattedTopicDefinition is null) return [];

        var topicItems = formattedTopicDefinition.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.Name.StartsWith("Item", StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.Name)
            .Select(x => x.GetValue(formattedTopicDefinition))
            .ToArray();

        var textParts = PlaceholderRegex.Split(formattedTopicDefinition.TopicDefinition.MessageFormat);
        var segments = new List<FormattedTopicSegment>(textParts.Length * 2);

        for (var i = 0; i < textParts.Length; i++) {
            segments.Add(new FormattedTopicSegment(textParts[i], false));

            if (topicItems.Length <= i) continue;

            segments.Add(new FormattedTopicSegment(Describe(topicItems[i])?.ToString() ?? string.Empty, true));
        }

        return segments;
    }

    private static object? Describe(object? item) => item switch {
        IMajorRecordIdentifierGetter recordIdentifier => recordIdentifier.EditorID ?? recordIdentifier.FormKey.ToString(),
        IFormLinkIdentifier formLinkIdentifier => ObjectConverters.GetStringValue(formLinkIdentifier),
        _ => item,
    };
}
