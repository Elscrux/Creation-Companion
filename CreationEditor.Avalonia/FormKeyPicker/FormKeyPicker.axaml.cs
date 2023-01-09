using Avalonia;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
namespace CreationEditor.Avalonia.FormKeyPicker;

// Ported from Mutagen.Bethesda.WPF by Noggog
public class FormKeyPicker : AFormKeyPicker {
    public double MaxSearchBoxHeight {
        get => GetValue(MaxSearchBoxHeightProperty);
        set => SetValue(MaxSearchBoxHeightProperty, value);
    }
    public static readonly StyledProperty<double> MaxSearchBoxHeightProperty = AvaloniaProperty.Register<FormKeyPicker, double>(nameof(MaxSearchBoxHeight), 1000d);

    public double SearchBoxHeight {
        get => GetValue(SearchBoxHeightProperty);
        set => SetValue(SearchBoxHeightProperty, value);
    }
    public static readonly StyledProperty<double> SearchBoxHeightProperty = AvaloniaProperty.Register<FormKeyPicker, double>(nameof(SearchBoxHeight), double.NaN);

    static FormKeyPicker() {
        // DefaultStyleKeyProperty.OverrideMetadata(typeof(FormKeyPicker), new FrameworkPropertyMetadata(typeof(FormKeyPicker))); todo
    }

    public FormKeyPicker() {
        PickerClickCommand = ReactiveCommand.Create((object o) => {
            if (o is not IMajorRecordIdentifier identifier) return;

            FormKey = identifier.FormKey;
            InSearchMode = false;
        });
    }
}
