using Avalonia.ReactiveUI;
using PromoteToMaster.ViewModels;
namespace PromoteToMaster.Views;

public partial class PromoteToMasterView : ReactiveUserControl<PromoteToMasterVM> {
    public PromoteToMasterView() {
        InitializeComponent();
    }

    public PromoteToMasterView(PromoteToMasterVM vm) : this() {
        DataContext = vm;
    }
}
