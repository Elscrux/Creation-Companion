using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia.Models.Docking;
using CreationEditor.Avalonia.ViewModels.Docking;
namespace CreationEditor.Avalonia.Views.Docking;

public partial class Tab : UserControl {
    public static readonly StyledProperty<IDockedItem> DockedItemProperty
        = AvaloniaProperty.Register<Tab, IDockedItem>(nameof(DockedItem));

    public IDockedItem DockedItem {
        get => GetValue(DockedItemProperty);
        set => SetValue(DockedItemProperty, value);
    }

    public static readonly StyledProperty<TabbedDockVM?> DockContainerProperty
        = AvaloniaProperty.Register<Tab, TabbedDockVM?>(nameof(DockContainer));

    public TabbedDockVM? DockContainer {
        get => GetValue(DockContainerProperty);
        set => SetValue(DockContainerProperty, value);
    }

    public Tab() {
        InitializeComponent();
    }
}
