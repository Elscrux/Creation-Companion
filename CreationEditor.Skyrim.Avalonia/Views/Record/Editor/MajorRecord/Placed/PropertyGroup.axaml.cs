using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Placed;

public partial class PropertyGroup : UserControl {
    public static readonly StyledProperty<bool> VisibleProperty
        = AvaloniaProperty.Register<PropertyGroup, bool>(nameof(Visible));
    public bool Visible {
        get => GetValue(VisibleProperty);
        set => SetValue(VisibleProperty, value);
    }

    public static readonly StyledProperty<AvaloniaList<object?>> ChildrenProperty
        = AvaloniaProperty.Register<PropertyGroup, AvaloniaList<object?>>(nameof(Children));
    [Content] public AvaloniaList<object?> Children {
        get => GetValue(ChildrenProperty);
        set => SetValue(ChildrenProperty, value);
    }

    public static readonly StyledProperty<string?> TitleProperty
        = AvaloniaProperty.Register<PropertyGroup, string?>(nameof(Title));
    public string? Title {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public PropertyGroup() {
        InitializeComponent();
    }
}
