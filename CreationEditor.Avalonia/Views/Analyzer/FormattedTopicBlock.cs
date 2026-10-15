using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Media;
using CreationEditor.Avalonia.Constants;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Analyzer;

/// <summary>
///     A piece of a topic's message: either literal text, or a value that filled one of its placeholders.
/// </summary>
public sealed record FormattedTopicSegment(string Text, bool IsItem);

/// <summary>
///     Renders a topic message, with each placeholder value shown as a click-to-copy button.
/// </summary>
public sealed class FormattedTopicBlock : TextBlock {
    public static readonly StyledProperty<IReadOnlyList<FormattedTopicSegment>?> SegmentsProperty =
        AvaloniaProperty.Register<FormattedTopicBlock, IReadOnlyList<FormattedTopicSegment>?>(nameof(Segments));

    public IReadOnlyList<FormattedTopicSegment>? Segments {
        get => GetValue(SegmentsProperty);
        set => SetValue(SegmentsProperty, value);
    }

    public FormattedTopicBlock() {
        VerticalAlignment = VerticalAlignment.Center;
        TextWrapping = TextWrapping.Wrap;
        FontSize = 12;
        Foreground = StandardBrushes.DarkGrayBrush;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == SegmentsProperty) {
            Rebuild();
        }
    }

    private void Rebuild() {
        var inlines = Inlines ??= new InlineCollection();
        inlines.Clear();

        if (Segments is null) return;

        foreach (var segment in Segments) {
            inlines.Add(segment.IsItem ? CreateItem(segment.Text) : CreateText(segment.Text));
        }
    }

    private static Run CreateText(string text) => new() {
        Text = text,
        FontSize = 12,
        BaselineAlignment = BaselineAlignment.Center,
    };

    private InlineUIContainer CreateItem(string text) => new(new Button {
        Content = text,
        FontSize = 12,
        Background = Brushes.Transparent,
        BorderThickness = new Thickness(0),
        Foreground = StandardBrushes.TextBrush,
        Margin = new Thickness(0),
        Padding = new Thickness(0, 2, 0, 0),
        VerticalAlignment = VerticalAlignment.Center,
        Command = ReactiveCommand.Create<string?>(Copy),
        CommandParameter = text,
        [ToolTip.TipProperty] = "Copy",
    });

    private void Copy(string? text) {
        if (TopLevel.GetTopLevel(this) is not { Clipboard: {} clipboard }) return;

        clipboard.SetTextAsync(text);
    }
}
