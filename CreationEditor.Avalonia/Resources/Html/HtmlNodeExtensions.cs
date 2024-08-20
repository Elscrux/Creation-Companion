using System.Diagnostics.CodeAnalysis;
using HtmlAgilityPack;
namespace CreationEditor.Avalonia.Html;

public static class HtmlNodeExtensions {
    public static bool TryGetAttribute<T>(this HtmlNode node, string attributeName, [MaybeNullWhen(false)] out T value)
        where T : IParsable<T> {
        var nodeAttribute = node.Attributes[attributeName];
        if (nodeAttribute is null) {
            value = default;
            return false;
        }

        return T.TryParse(nodeAttribute.Value, null, out value);
    }
}
