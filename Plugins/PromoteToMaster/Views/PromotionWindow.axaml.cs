using System.Reactive.Linq;
using CreationEditor.Avalonia.Views;
using Noggog;
using PromoteToMaster.ViewModels;
using ReactiveUI;
namespace PromoteToMaster.Views;

public partial class PromotionWindow : ActivatableAppWindow {
    public PromotionWindow() {
        InitializeComponent();
    }

    protected override void WhenActivated() {
        base.WhenActivated();

        this.WhenAnyValue(x => x.DataContext)
            .NotNull()
            .OfType<PromoteToMasterVM>()
            .Subscribe(vm => {
                Title = vm.RecordsToPromote.Count > 1
                    ? $"Promote {vm.RecordsToPromote.Count} Records"
                    : $"Promote {vm.RecordsToPromote[0].Record.EditorID ?? vm.RecordsToPromote[0].Record.FormKey.ToString()}";

                vm.Run.Subscribe(_ => {
                    Close();
                });
            })
            .DisposeWith(ActivatedDisposable);
    }
}
