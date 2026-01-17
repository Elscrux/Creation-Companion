using Avalonia.ReactiveUI;
using LeveledList.ViewModels;

namespace LeveledList.Views;

public partial class TiersView : ReactiveUserControl<TiersVM> {
    public TiersView() {
        InitializeComponent();
    }
}
