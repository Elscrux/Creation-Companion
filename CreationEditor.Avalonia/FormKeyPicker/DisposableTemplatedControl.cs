using Avalonia.Controls.Primitives;
using Noggog;
namespace CreationEditor.Avalonia.FormKeyPicker;

public class DisposableTemplatedControl : TemplatedControl {
    protected readonly IDisposableBucket UnloadDisposable = new DisposableBucket();
    protected readonly IDisposableBucket TemplateDisposable = new DisposableBucket();

    public DisposableTemplatedControl() {
        Unloaded += (_, _) => UnloadDisposable.Clear();
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        TemplateDisposable.Clear();
    }
}