using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;
using CreationEditor.Avalonia.Attached.DragDrop;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Views.Record.Picker;

// Ported from Mutagen.Bethesda.WPF by Noggog
[TemplatePart(Name = PopupName, Type = typeof(Popup))]
public class FormKeyPicker : AFormKeyPicker {
    private const string PopupName = "PART_Popup";

    public double MaxSearchBoxHeight {
        get => GetValue(MaxSearchBoxHeightProperty);
        set => SetValue(MaxSearchBoxHeightProperty, value);
    }
    public static readonly StyledProperty<double> MaxSearchBoxHeightProperty =
        AvaloniaProperty.Register<FormKeyPicker, double>(nameof(MaxSearchBoxHeight), 500d);

    public double MinSearchBoxWidth {
        get => GetValue(MinSearchBoxWidthProperty);
        set => SetValue(MinSearchBoxWidthProperty, value);
    }
    public static readonly StyledProperty<double> MinSearchBoxWidthProperty =
        AvaloniaProperty.Register<FormKeyPicker, double>(nameof(MinSearchBoxWidth), 250d);

    public double SearchBoxHeight {
        get => GetValue(SearchBoxHeightProperty);
        set => SetValue(SearchBoxHeightProperty, value);
    }
    public static readonly StyledProperty<double> SearchBoxHeightProperty =
        AvaloniaProperty.Register<FormKeyPicker, double>(nameof(SearchBoxHeight), double.NaN);

    public bool ShowFormKeyBox {
        get => GetValue(ShowFormKeyBoxProperty);
        set => SetValue(ShowFormKeyBoxProperty, value);
    }
    public static readonly StyledProperty<bool> ShowFormKeyBoxProperty = AvaloniaProperty.Register<FormKeyPicker, bool>(nameof(ShowFormKeyBox), true);

    private Popup? _popup;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _popup = e.NameScope.Find<Popup>(PopupName);

        SetValue(DragDropExtended.DragHandlerProperty, new CustomDragDropDataHandler<FormLinkDragDrop, IFormLinkIdentifier>());

        SetValue(FormLinkDragDrop.GetFormLinkProperty,
            _ => {
                if (LinkCache is null
                 || !LinkCache.TryResolve(FormKey, EnabledTypes(SelectableTypes), out var record))
                    return FormLinkInformation.Null;

                return FormLinkInformation.Factory(record);
            });

        SetValue(FormLinkDragDrop.SetFormLinkProperty, formLink => FormKey = formLink.FormKey);

        SetValue(
            FormLinkDragDrop.CanSetFormLinkProperty,
            formLink => {
                // FormLink type needs to be in scoped type
                var selectedTypes = EnabledTypes(SelectableTypes).ToList();
                if (!selectedTypes.AnyInheritsFrom(typeof(IMajorRecordGetter))
                 && !selectedTypes.AnyInheritsFrom(formLink.Type)) return false;

                // FormKey must not be blacklisted
                if (BlacklistFormKeys is not null && BlacklistFormKeys.Contains(formLink.FormKey)) return false;

                // FormKey must be resolved
                if (LinkCache is null || !LinkCache.TryResolveIdentifier(formLink.FormKey, selectedTypes, out var editorId)) return false;

                // Record needs to satisfy the filter
                if (Filter is not null && !Filter(formLink.FormKey, editorId)) return false;

                return true;
            });
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e) {
        base.OnPointerReleased(e);

        if (e is not { Handled: false, Source: Visual source }) return;
        if (_popup?.IsInsidePopup(source) is not true) return;

        var border = source.FindAncestorOfType<Border>(true);
        if (border?.DataContext is not RecordNamePair identifierCustomName) return;

        FormKey = identifierCustomName.Record.FormKey;

        var focusManager = TopLevel.GetTopLevel(this)?.FocusManager;
        focusManager?.ClearFocus();

        InSearchMode = false;
    }
}
