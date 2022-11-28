using Avalonia.Controls.Primitives;
using Noggog;
namespace CreationEditor.WPF.FormKeyPicker;

public class DisposableTemplatedControl : TemplatedControl {
    protected readonly IDisposableBucket UnloadDisposable = new DisposableBucket();
    protected readonly IDisposableBucket TemplateDisposable = new DisposableBucket();

    public DisposableTemplatedControl() {
        Loaded += (_, _) => OnLoaded();
        Loaded += (_, _) => {
            if (Template == null) return;

            TemplateDisposable.Clear();
        };
        Unloaded += (_, _) => {
            TemplateDisposable.Clear();
            UnloadDisposable.Clear();
        };
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        TemplateDisposable.Clear();
    }

    protected override void OnLoaded() {}
}