using Avalonia;
using Avalonia.Input;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Avalonia.Attached.DragDrop;

public sealed record AssetDataSourceLink(IDataSourceLink DataSourceLink, IAssetLinkGetter AssetLink);

public sealed class AssetLinkDragDrop : AvaloniaObject, ICustomDragDropData<AssetDataSourceLink> {
    public static readonly AttachedProperty<Func<object?, AssetDataSourceLink>?> GetAssetLinkProperty =
        AvaloniaProperty.RegisterAttached<AssetLinkDragDrop, InputElement, Func<object?, AssetDataSourceLink>?>("GetAssetLink");
    public static readonly AttachedProperty<Action<AssetDataSourceLink>?> SetAssetLinkProperty =
        AvaloniaProperty.RegisterAttached<AssetLinkDragDrop, InputElement, Action<AssetDataSourceLink>?>("SetAssetLink");

    public static readonly AttachedProperty<Func<AssetDataSourceLink, bool>?> CanSetAssetLinkProperty =
        AvaloniaProperty.RegisterAttached<AssetLinkDragDrop, InputElement, Func<AssetDataSourceLink, bool>?>("CanSetAssetLink");

    public static Func<object?, AssetDataSourceLink>? GetGetAssetLink(AvaloniaObject obj) => obj.GetValue(GetAssetLinkProperty);
    public static void SetGetAssetLink(AvaloniaObject obj, Func<object?, AssetDataSourceLink> value) => obj.SetValue(GetAssetLinkProperty, value);

    public static Action<AssetDataSourceLink>? GetSetAssetLink(AvaloniaObject obj) => obj.GetValue(SetAssetLinkProperty);
    public static void SetSetAssetLink(AvaloniaObject obj, Action<AssetDataSourceLink>? value) => obj.SetValue(SetAssetLinkProperty, value);

    public static Func<AssetDataSourceLink, bool>? GetCanSetAssetLink(AvaloniaObject obj) => obj.GetValue(CanSetAssetLinkProperty);
    public static void SetCanSetAssetLink(AvaloniaObject obj, Func<AssetDataSourceLink, bool>? value) => obj.SetValue(CanSetAssetLinkProperty, value);
    
    public static string Data => "AssetLink";
    public static Func<object?, AssetDataSourceLink>? GetData(AvaloniaObject obj) => GetGetAssetLink(obj);
    public static Action<AssetDataSourceLink>? GetSetData(AvaloniaObject obj) => GetSetAssetLink(obj);
    public static Func<AssetDataSourceLink, bool>? GetCanSetData(AvaloniaObject obj) => GetCanSetAssetLink(obj);
}
