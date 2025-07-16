namespace LeveledList.ViewModels;

public class LeveledListVM(ListsVM listsVM, TiersVM tiersVM, EnchantmentsVM enchantmentsVM) {
    public ListsVM ListsVM { get; } = listsVM;
    public TiersVM TiersVM { get; } = tiersVM;
    public EnchantmentsVM EnchantmentsVM { get; } = enchantmentsVM;
}
