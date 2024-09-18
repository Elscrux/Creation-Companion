using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
namespace CreationEditor.Avalonia.Views.Basic;

[TemplatePart(LabelPart, typeof(TextBlock))]
public class LabeledContentControl : HeaderedContentControl {
    public const string LabelPart = "PART_Label";

    public static readonly StyledProperty<object?> HeaderToolTipProperty
        = AvaloniaProperty.Register<LabeledContentControl, object?>(nameof(HeaderToolTip));

    public object? HeaderToolTip {
        get => GetValue(HeaderToolTipProperty);
        set => SetValue(HeaderToolTipProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        var label = e.NameScope.Find<TextBlock>(LabelPart);

        if (label == null) return;

        label.PointerPressed -= OnLabelPressed;
        label.PointerPressed += OnLabelPressed;
    }

    private void LabelActivated(PointerPressedEventArgs args) {
        if (Presenter is null) {
            args.Handled = false;
        } else {
            Presenter.Focus();
            if (Presenter.Child is ToggleButton { IsEnabled: true } checkBox) {
                checkBox.IsChecked = !checkBox.IsChecked;
            }
        }
    }

    private void OnLabelPressed(object? sender, PointerPressedEventArgs args) {
        if (args.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed) {
            LabelActivated(args);
        }
        base.OnPointerPressed(args);
    }
}
