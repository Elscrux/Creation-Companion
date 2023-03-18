using Avalonia.Controls;
using Noggog;
namespace CreationEditor.Avalonia.Views; 

public abstract class LoadedUserControl : UserControl {
    protected readonly IDisposableBucket UnloadDisposable = new DisposableBucket();

    protected override void OnUnloaded() {
        base.OnUnloaded();

        UnloadDisposable.Clear();
    }
}
