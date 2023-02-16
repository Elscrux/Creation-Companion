using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.FormKeyPicker;

// Ported from Mutagen.Bethesda.WPF by Noggog
[TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
public class FormKeyPicker : AFormKeyPicker {
    public double MaxSearchBoxHeight {
        get => GetValue(MaxSearchBoxHeightProperty);
        set => SetValue(MaxSearchBoxHeightProperty, value);
    }
    public static readonly StyledProperty<double> MaxSearchBoxHeightProperty = AvaloniaProperty.Register<FormKeyPicker, double>(nameof(MaxSearchBoxHeight), 500d);
    
    public double MinSearchBoxWidth {
        get => GetValue(MinSearchBoxWidthProperty);
        set => SetValue(MinSearchBoxWidthProperty, value);
    }
    public static readonly StyledProperty<double> MinSearchBoxWidthProperty = AvaloniaProperty.Register<FormKeyPicker, double>(nameof(MinSearchBoxWidth), 250d);

    public double SearchBoxHeight {
        get => GetValue(SearchBoxHeightProperty);
        set => SetValue(SearchBoxHeightProperty, value);
    }
    public static readonly StyledProperty<double> SearchBoxHeightProperty = AvaloniaProperty.Register<FormKeyPicker, double>(nameof(SearchBoxHeight), double.NaN);
    
    private Popup? _popup;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        
        _popup = e.NameScope.Find<Popup>("PART_Popup");
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e) {
        base.OnPointerReleased(e);

        if (e is not { Handled: false, Source: IVisual source }) return;
        if (_popup?.IsInsidePopup(source) is not true) return;

        var border = source.FindAncestorOfType<Border>(true);
        if (border?.DataContext is not IMajorRecordIdentifier identifier) return;

        FormKey = identifier.FormKey;
        
        // Unfocus twice to ensure the text box doesn't get focus again
        FocusManager.Instance?.Focus(null);
        FocusManager.Instance?.Focus(null);
        
        InSearchMode = false;
    }
}
