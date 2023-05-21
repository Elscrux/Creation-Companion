using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using FluentAvalonia.UI.Windowing;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Views.Reference;

public partial class ReferenceWindow : AppWindow {
    public static readonly StyledProperty<IEnumerable<TabItem>> TabsProperty
        = AvaloniaProperty.Register<ReferenceWindow, IEnumerable<TabItem>>(nameof(Tabs));

    public IEnumerable<TabItem> Tabs {
        get => GetValue(TabsProperty);
        set => SetValue(TabsProperty, value);
    }

    public ReferenceWindow() {
        InitializeComponent();
    }

    public ReferenceWindow(string name) : this() {
        Title = $"References of {name}";
    }

    public ReferenceWindow(string name, params TabItem[] tabs) : this(name) {
        Tabs = tabs;
    }

    public ReferenceWindow(string name, IEnumerable<TabItem> tabs) : this(name) {
        Tabs = tabs;
    }

    public ReferenceWindow(IMajorRecordIdentifier record) : this() {
        var editorId = record.EditorID;
        Title = $"References of {record.FormKey}" + (editorId == null ? string.Empty : $" - {editorId}");
    }
}
