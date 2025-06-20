using Avalonia.Data.Converters;
using Avalonia.ReactiveUI;
using PromoteToMaster.ViewModels;
namespace PromoteToMaster.Views;

public partial class PromoteToMasterView : ReactiveUserControl<PromoteToMasterVM> {
    public static readonly FuncValueConverter<AssetPromotionMode, string> AssetPromotionModeToString
        = new(mode =>
            mode switch {
                AssetPromotionMode.Nothing => "Nothing will be moved",
                AssetPromotionMode.Copy => "All assets referenced by the records will be copied over to the 'To' data source",
                AssetPromotionMode.Move => "The assets referenced by the records will be moved over in case they are not referenced by anything in the 'From' data sources anymore, otherwise they will be copied",
                AssetPromotionMode.ForceMove => "All assets referenced by the records will be moved over regardless if they are still referenced by something in the 'From' data sources",
                _ => string.Empty,
            }
        );

    public PromoteToMasterView() {
        InitializeComponent();
    }

    public PromoteToMasterView(PromoteToMasterVM vm) : this() {
        DataContext = vm;
    }
}
