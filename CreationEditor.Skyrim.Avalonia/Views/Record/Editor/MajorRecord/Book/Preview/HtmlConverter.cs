using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.Media;
using CreationEditor.Avalonia.Html;
using CreationEditor.Avalonia.Services.Asset;
using CreationEditor.Avalonia.Services.Avalonia.Font;
using HtmlAgilityPack;
using Mutagen.Bethesda.Fonts;
using Mutagen.Bethesda.Strings;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Book.Preview;

public sealed record HtmlConverterOptions(Language Language, double Width, double LineSpacing, string DefaultFont = "SkyrimBooks_Gaelic");

public sealed class HtmlConverter(
    IImageLoader imageLoader,
    IGameFontLoader gameFontLoader,
    Func<Language, IFontProvider> fontProviderFactory,
    HtmlConverterOptions htmlConverterOptions) {
    private readonly IFontProvider _fontProvider = fontProviderFactory(htmlConverterOptions.Language);

    public IEnumerable<Control> GenerateControls(string text) {
        var currentStackPanel = new StackPanel();
        foreach (var control in GetControls(text)) {
            // for every separator in the list, create a new stack panel
            if (control is Separator) {
                if (currentStackPanel.Children.Count == 0) {
                    currentStackPanel.Children.Add(new TextBlock());
                }
                yield return currentStackPanel;

                currentStackPanel = new StackPanel();
            } else {
                currentStackPanel.Children.Add(control);
            }
        }

        yield return currentStackPanel;
    }

    private IEnumerable<Control> GetControls(string pageText) {
        var doc = new HtmlDocument();
        doc.LoadHtml(pageText.Replace("\r", string.Empty));
        PreProcess(doc);

        foreach (var handleNode in GetNodes(doc.DocumentNode.ChildNodes, new TextOptions(htmlConverterOptions.DefaultFont))) {
            yield return handleNode;
        }
    }

    private static void PreProcess(HtmlDocument doc) {
        foreach (var node in doc.DocumentNode.GetAllChildren(node => node.ChildNodes, true)) {
            for (var i = 0; i < node.ChildNodes.Count; i++) {
                var currentNode = node.ChildNodes[i];
                if (currentNode.Name is not ("img" or "font" or "p" or "b" or "i" or "u")) continue;

                if (currentNode.Name != "p" && i < node.ChildNodes.Count - 1) {
                    var nextNode = node.ChildNodes[i + 1];
                    if (nextNode.InnerHtml.StartsWith('\n')) {
                        nextNode.InnerHtml = nextNode.InnerHtml[1..];
                    } else {
                        currentNode.Attributes.Append(LinkWithNext);
                    }
                }

                if (i > 0) {
                    var previousNode = node.ChildNodes[i - 1];
                    if (previousNode.InnerHtml.EndsWith('\n')) {
                        previousNode.InnerHtml = previousNode.InnerHtml[..^1];
                    } else if (currentNode.Name != "p") {
                        currentNode.Attributes.Append(LinkWithPrevious);
                    }
                }
            }
        }
    }

    private const string LinkWithPrevious = "_link_with_previous";
    private const string LinkWithNext = "_link_with_next";

    private IEnumerable<Control> GetNodes(HtmlNodeCollection nodeCollection, TextOptions textOptions) {
        List<Control> leftoverControls = [];

        // create horizontal stack panel for nodes that have the _should_be_horizontal attribute
        for (var i = 0; i < nodeCollection.Count; i++) {
            var currentNode = nodeCollection[i];
            var currentControls = GetNode(currentNode, textOptions).ToList();

            var linkWithPrevious = currentNode.Attributes.Contains(LinkWithPrevious);
            var linkWithNext = currentNode.Attributes.Contains(LinkWithNext);

            if (linkWithPrevious || linkWithNext) {
                // Handle previous controls
                List<Control> previousControls;
                if (!linkWithPrevious) {
                    // In case the previous node is not linked, add all its content manually
                    foreach (var control in leftoverControls) {
                        yield return control;
                    }
                    previousControls = [];
                    leftoverControls = [];
                } else {
                    // In case the previous node is linked, add all its content except the last element which should be linked
                    foreach (var control in leftoverControls.SkipLast(1)) {
                        yield return control;
                    }
                    previousControls = leftoverControls.Count > 0 ? [leftoverControls[^1]] : [];
                    leftoverControls = [];
                }

                // Handle current controls - only use controls that are not separated by a line break
                var currentInlinedControls = currentControls.TakeWhile(c => c.Tag is not true).ToList();
                var inlined = previousControls.Concat(currentInlinedControls).ToList();

                // Handle next controls
                var counter = i;
                while (currentControls.Count == currentInlinedControls.Count
                 && counter < nodeCollection.Count - 1
                 && (currentNode.Attributes.Contains(LinkWithNext) || nodeCollection[counter + 1].Attributes.Contains(LinkWithPrevious))) {
                    // Current controls are fully part of the inline, take controls from the next node
                    counter++;
                    currentNode = nodeCollection[counter];
                    currentControls = GetNode(currentNode, textOptions).ToList();
                    currentInlinedControls = currentControls.TakeWhile(c => c.Tag is not true).ToList();
                    inlined.AddRange(currentInlinedControls);
                    if (currentControls.Count != currentInlinedControls.Count) {
                        leftoverControls = currentControls[currentInlinedControls.Count..].ToList();
                        break;
                    }
                }

                // Perform inlining
                foreach (var inline in ProcessInlines(inlined, textOptions)) {
                    yield return inline;
                }

                // Handle leftover controls in case no inlining was needed
                if (i == counter) {
                    leftoverControls = currentControls.SkipWhile(c => c.Tag is not true).ToList();
                }

                continue;
            }

            // When there is no inlining needed, just return the controls
            foreach (var control in leftoverControls) {
                yield return control;
            }
            leftoverControls = currentControls;
        }

        foreach (var control in leftoverControls) {
            yield return control;
        }
    }

    private IEnumerable<Control> ProcessInlines(IEnumerable<Control> inlineControls, TextOptions textOptions) {
        // Create text block inlines for nodes that are supposed to be linked together

        // Create inlined text block for the linked content
        var inlinedTextBlock = new TextBlock {
            MinWidth = htmlConverterOptions.Width,
            Foreground = textOptions.FontColor,
            HorizontalAlignment = textOptions.Alignment,
            VerticalAlignment = VerticalAlignment.Bottom,
            TextWrapping = TextWrapping.Wrap,
            LineSpacing = htmlConverterOptions.LineSpacing,
            Inlines = new InlineCollection(),
            Tag = false,
        };

        // Add all controls to the inlined text block
        foreach (var control in inlineControls) {
            switch (control) {
                case TextBlock { Inlines: { Count: > 0 } inlines }:
                    inlinedTextBlock.Inlines.AddRange(inlines);
                    break;
                case TextBlock text:
                    if (text.Text is null || text.Text.Length == 0) break;

                    inlinedTextBlock.Inlines.Add(new Run {
                        Theme = text.Theme,
                        Background = text.Background,
                        FontFamily = text.FontFamily,
                        FontSize = text.FontSize,
                        FontStyle = text.FontStyle,
                        FontWeight = text.FontWeight,
                        FontStretch = text.FontStretch,
                        Foreground = text.Foreground,
                        TextDecorations = text.TextDecorations,
                        BaselineAlignment = BaselineAlignment.Bottom,
                        Text = text.Text,
                    });
                    break;
                default:
                    inlinedTextBlock.Inlines.Add(new InlineUIContainer(control) {
                        BaselineAlignment = BaselineAlignment.Bottom,
                    });
                    break;
            }
        }

        // Return the inlined text block
        if (inlinedTextBlock.Inlines.Count > 0) {
            yield return inlinedTextBlock;
        }
    }

    private IEnumerable<Control> GetNode(HtmlNode node, TextOptions textOptions) {
        switch (node.NodeType) {
            case HtmlNodeType.Document:
                break;
            case HtmlNodeType.Element:
                switch (node.Name) {
                    case "font":
                        return ProcessFont(node, textOptions);
                    case "p":
                        return ProcessParagraph(node, textOptions);
                    case "b":
                        return GetNodes(node.ChildNodes, textOptions with { IsBold = true });
                    case "i":
                        return GetNodes(node.ChildNodes, textOptions with { IsItalic = true });
                    case "u":
                        return GetNodes(node.ChildNodes, textOptions with { IsUnderlined = true });
                    case "img":
                        return ProcessImage(node, textOptions);
                    case "br":
                        return ProcessText("\n", textOptions);
                    default:
                        Console.WriteLine("unknown ELEMENT with name " + node.Name + " and value" + node.InnerText);
                        break;
                }
                break;
            case HtmlNodeType.Text:
                return ProcessText(node.InnerText, textOptions);
            case HtmlNodeType.Comment:
                break;
            default:
                throw new InvalidOperationException();
        }

        return GetNodes(node.ChildNodes, textOptions);
    }

    private IEnumerable<Control> ProcessText(string nodeText, TextOptions textOptions) {
        // Handle spaces before and after [pagebreak]
        const string pagebreak = "[pagebreak]";
        while (nodeText.Contains(" " + pagebreak, StringComparison.OrdinalIgnoreCase))
            nodeText = nodeText.Replace(" " + pagebreak, pagebreak, StringComparison.OrdinalIgnoreCase);
        while (nodeText.Contains(pagebreak + " ", StringComparison.OrdinalIgnoreCase))
            nodeText = nodeText.Replace(pagebreak + " ", pagebreak, StringComparison.OrdinalIgnoreCase);

        // Add newlines around pagebreaks to ensure the split works in all valid cases
        if (nodeText.StartsWith(pagebreak, StringComparison.OrdinalIgnoreCase)) nodeText = "\n" + nodeText;
        if (nodeText.EndsWith(pagebreak, StringComparison.OrdinalIgnoreCase)) nodeText += "\n";

        return nodeText
            .Split($"\n{pagebreak}\n")
            .SelectMany(GetTextBlock);

        IEnumerable<Control> GetTextBlock(string text, int i) {
            if (i != 0) {
                // At the start of a new page, add a separator
                yield return new Separator();

                if (string.IsNullOrEmpty(text)) yield break;
            }

            // Splitting on \n will create two text blocks instead of one
            // This prevents this
            if (text == "\n") text = string.Empty;

            foreach (var textBlock in text
                .Split("\n")
                .Select((t, p) => new TextBlock {
                    Foreground = textOptions.FontColor,
                    HorizontalAlignment = textOptions.Alignment,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    FontSize = textOptions.FontSize,
                    FontStyle = textOptions.IsItalic ? FontStyle.Italic : FontStyle.Normal,
                    FontFamily = textOptions.FontFamily,
                    FontWeight = textOptions.IsBold ? FontWeight.Bold : FontWeight.Normal,
                    TextDecorations = textOptions.IsUnderlined ? [new TextDecoration { Location = TextDecorationLocation.Underline }] : [],
                    Text = t,
                    Tag = p > 0, // determines if the text block appears after a line break
                    TextWrapping = TextWrapping.Wrap,
                    LineSpacing = htmlConverterOptions.LineSpacing,
                })) {
                yield return textBlock;
            }
        }
    }

    private IEnumerable<Control> ProcessParagraph(HtmlNode node, TextOptions textOptions) {
        if (node.TryGetAttribute<string>("align", out var align)) {
            var horizontalAlignment = align switch {
                "left" => HorizontalAlignment.Left,
                "center" => HorizontalAlignment.Center,
                "right" => HorizontalAlignment.Right,
                _ => HorizontalAlignment.Left,
            };

            textOptions = textOptions with { Alignment = horizontalAlignment };
        }

        return GetNodes(node.ChildNodes, textOptions);
    }

    private IEnumerable<Control> ProcessFont(HtmlNode node, TextOptions textOptions) {
        if (node.TryGetAttribute<string>("color", out var colorStr) && Color.TryParse(colorStr, out var color)) {
            textOptions = textOptions with { FontColor = new SolidColorBrush(color) };
        }
        if (node.TryGetAttribute<string>("face", out var faceStr)) {
            if (_fontProvider.FontMappings.TryGetValue(faceStr, out var fontName)) {
                var fontFamily = new FontFamily(gameFontLoader.GetFontName(fontName.FontId));
                textOptions = textOptions with { FontFamily = fontFamily };
            }
        }
        if (node.TryGetAttribute<int>("size", out var size)) {
            textOptions = textOptions with { FontSize = size <= 0 ? 1 : size };
        }
        if (node.TryGetAttribute<string>("alpha", out var hexAlpha)
         && int.TryParse(hexAlpha.TrimStart('#'), NumberStyles.HexNumber, null, out var alpha)) {
            textOptions = textOptions with { FontColor = new SolidColorBrush(textOptions.FontColor.Color, alpha / 255.0) };
        }
        return GetNodes(node.ChildNodes, textOptions);
    }

    private IEnumerable<Control> ProcessImage(HtmlNode imageNode, TextOptions textOptions) {
        const string imagePrefix = "img://";
        var src = imageNode.GetAttributeValue("src", string.Empty);
        if (!src.StartsWith(imagePrefix, StringComparison.OrdinalIgnoreCase)) return [];

        return [
            new Image {
                Source = imageLoader.LoadImage(src[imagePrefix.Length..]),
                HorizontalAlignment = textOptions.Alignment,
                VerticalAlignment = VerticalAlignment.Bottom,
                Stretch = Stretch.Fill,
                Width = imageNode.TryGetAttribute<int>("width", out var w) ? w : 64,
                Height = imageNode.TryGetAttribute<int>("height", out var h) ? h : 64,
            },
        ];
    }

    private sealed record TextOptions {
        public TextOptions(string font) => FontFamily = FontFamily.Parse(font);
        public FontFamily FontFamily { get; init; }
        public HorizontalAlignment Alignment { get; init; } = HorizontalAlignment.Left;
        public bool IsBold { get; init; }
        public bool IsItalic { get; init; }
        public bool IsUnderlined { get; init; }
        public int FontSize { get; init; } = 22;
        public ISolidColorBrush FontColor { get; init; } = Brushes.Black;
    }
}
