using Avalonia;
using Avalonia.Controls;
namespace CreationEditor.Avalonia.Views.Reference;

public partial class ReferenceList : UserControl {
    public static readonly StyledProperty<IEnumerable<TabItem>> TabsProperty
        = AvaloniaProperty.Register<ReferenceList, IEnumerable<TabItem>>(nameof(Tabs));

    public IEnumerable<TabItem> Tabs {
        get => GetValue(TabsProperty);
        set => SetValue(TabsProperty, value);
    }

    public ReferenceList() {
        InitializeComponent();
    }

    public ReferenceList(params IEnumerable<TabItem> tabs) : this() {
        Tabs = tabs;
    }
}
