namespace LeveledList.ViewModels;

public class LeveledListVM(ListsVM listsVM, TiersVM tiersVM) {
    public ListsVM ListsVM { get; } = listsVM;
    public TiersVM TiersVM { get; } = tiersVM;
}
