using System.Windows;
using System.Windows.Controls;
namespace CreationEditor.WPF.ViewModels.Docking.Selectors;

public class PanesStyleSelector : StyleSelector {
    public Style GenericStyle { get; set; } = null!;
    public Style RecordStyle { get; set; } = null!;

    public override Style SelectStyle(object item, DependencyObject container) {
        return item is RecordPaneVM ? RecordStyle : GenericStyle;
    }
}