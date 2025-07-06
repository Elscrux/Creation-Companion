using Avalonia.ReactiveUI;
using LeveledList.ViewModels;

namespace LeveledList.Views;

public partial class LeveledListView : ReactiveUserControl<LeveledListVM> {
    public LeveledListView() {
        InitializeComponent();
    }

    public LeveledListView(LeveledListVM listsVM) : this() {
        DataContext = listsVM;
    }
}
