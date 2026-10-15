using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;
namespace CreationEditor.Avalonia.Views.Basic;

[TemplatePart(LabelPart, typeof(ContentControl))]
public class LabeledContentControl : HeaderedContentControl {
    public const string LabelPart = "PART_Label";

    private static readonly FuncDataTemplate DefaultHeaderTemplate = new(
        static header => header is not Control,
        static (_, _) => new TextBlock {
            [!TextBlock.TextProperty] = new Binding(""),
            FontSize = 16,
            FontWeight = FontWeight.SemiBold,
        });

    public static readonly StyledProperty<object?> HeaderToolTipProperty
        = AvaloniaProperty.Register<LabeledContentControl, object?>(nameof(HeaderToolTip));

    public object? HeaderToolTip {
        get => GetValue(HeaderToolTipProperty);
        set => SetValue(HeaderToolTipProperty, value);
    }

    public LabeledContentControl() => HeaderTemplate = DefaultHeaderTemplate;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == HeaderProperty) {
            // If the header is a control, we don't want to apply the default text template
            HeaderTemplate = Header is Control ? null : DefaultHeaderTemplate;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        var label = e.NameScope.Find<ContentControl>(LabelPart);

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
