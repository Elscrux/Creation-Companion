using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Mod;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Mod;

public partial class ModSelectionPopup : ReactiveUserControl<ModSelectionVM> {
    public ModSelectionPopup() {
        InitializeComponent();

        this.WhenActivated(disposables => {
            this.WhenAnyValue(x => x.ViewModel)
                .NotNull()
                .Subscribe(vm => {
                    LoadButton[!ContentProperty] = vm.AnyModsActive
                        .Select(anyModActive => anyModActive ? "Load" : "Create")
                        .ToBinding();
                })
                .DisposeWith(disposables);
        });
    }

    public ModSelectionPopup(ModSelectionVM modSelectionVM) : this() {
        DataContext = modSelectionVM;
    }

    public void Show() => IsVisible = true;
    public void Hide() {
        IsVisible = false;
        CancelButton.IsEnabled = true;
    }

    public async Task LoadAndHide() {
        if (ViewModel is not null && await ViewModel.LoadMods()) {
            Hide();
        }
    }
}
