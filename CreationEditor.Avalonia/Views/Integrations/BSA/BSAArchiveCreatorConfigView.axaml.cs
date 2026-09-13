using CreationEditor.Avalonia.ViewModels.Integrations.BSA;
using ReactiveUI.Avalonia;
namespace CreationEditor.Avalonia.Views.Integrations.BSA;

public partial class BSAArchiveCreatorConfigView : ReactiveUserControl<BSAArchiveCreatorConfigVM> {
    public BSAArchiveCreatorConfigView() {
        InitializeComponent();
    }

    public BSAArchiveCreatorConfigView(BSAArchiveCreatorConfigVM vm) : this() {
        DataContext = vm;
    }
}
