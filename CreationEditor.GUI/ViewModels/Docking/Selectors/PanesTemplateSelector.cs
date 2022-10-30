using System.Windows;
using System.Windows.Controls;
namespace CreationEditor.GUI.ViewModels.Docking.Selectors;

public class PanesTemplateSelector : DataTemplateSelector {
    public DataTemplate ContentControl { get; set; } = null!;
    public DataTemplate PaneTemplate { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container) {
        return item is PaneVM ? PaneTemplate : ContentControl;
    }
}