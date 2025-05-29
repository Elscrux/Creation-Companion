using Avalonia;
using CreationEditor.Avalonia.ViewModels.Record.Browser;
using CreationEditor.Services.Filter;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Record;

public partial class RecordBrowserSettingsView : ActivatableUserControl {
    public static readonly StyledProperty<RecordBrowserSettings?> SettingsProperty
        = AvaloniaProperty.Register<RecordBrowserSettingsView, RecordBrowserSettings?>(nameof(Settings));

    public RecordBrowserSettings? Settings {
        get => GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    public static readonly StyledProperty<bool> OnlyActiveProperty
        = AvaloniaProperty.Register<RecordBrowserSettingsView, bool>(nameof(OnlyActive));

    public bool OnlyActive {
        get => GetValue(OnlyActiveProperty);
        set => SetValue(OnlyActiveProperty, value);
    }

    public RecordBrowserSettingsView() {
        InitializeComponent();
    }

    protected override void WhenActivated() {
        base.WhenActivated();

        this.WhenAnyValue(x => x.OnlyActive)
            .ObserveOnGui()
            .Subscribe(_ => {
                if (Settings is null) return;

                Settings.ModScopeProviderVM.Scope = OnlyActive ? BrowserScope.ActiveMod : BrowserScope.Environment;
            })
            .DisposeWith(ActivatedDisposable);
    }
}
