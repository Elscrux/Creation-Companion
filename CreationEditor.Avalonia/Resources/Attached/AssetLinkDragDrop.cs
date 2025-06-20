using Avalonia;
using Avalonia.Input;
using CreationEditor.Services.DataSource;
using Mutagen.Bethesda.Plugins.Assets;
namespace CreationEditor.Avalonia.Attached;

public sealed record AssetFileSystemLink(FileSystemLink FileSystemLink, IAssetLinkGetter AssetLink);

public sealed class AssetLinkDragDrop : AvaloniaObject, ICustomDragDropData<AssetFileSystemLink> {
    public static readonly AttachedProperty<Func<object?, AssetFileSystemLink>?> GetAssetLinkProperty =
        AvaloniaProperty.RegisterAttached<AssetLinkDragDrop, InputElement, Func<object?, AssetFileSystemLink>?>("GetAssetLink");
    public static readonly AttachedProperty<Action<AssetFileSystemLink>?> SetAssetLinkProperty =
        AvaloniaProperty.RegisterAttached<AssetLinkDragDrop, InputElement, Action<AssetFileSystemLink>?>("SetAssetLink");

    public static readonly AttachedProperty<Func<AssetFileSystemLink, bool>?> CanSetAssetLinkProperty =
        AvaloniaProperty.RegisterAttached<AssetLinkDragDrop, InputElement, Func<AssetFileSystemLink, bool>?>("CanSetAssetLink");

    public static Func<object?, AssetFileSystemLink>? GetGetAssetLink(AvaloniaObject obj) => obj.GetValue(GetAssetLinkProperty);
    public static void SetGetAssetLink(AvaloniaObject obj, Func<object?, AssetFileSystemLink> value) => obj.SetValue(GetAssetLinkProperty, value);

    public static Action<AssetFileSystemLink>? GetSetAssetLink(AvaloniaObject obj) => obj.GetValue(SetAssetLinkProperty);
    public static void SetSetAssetLink(AvaloniaObject obj, Action<AssetFileSystemLink>? value) => obj.SetValue(SetAssetLinkProperty, value);

    public static Func<AssetFileSystemLink, bool>? GetCanSetAssetLink(AvaloniaObject obj) => obj.GetValue(CanSetAssetLinkProperty);
    public static void SetCanSetAssetLink(AvaloniaObject obj, Func<AssetFileSystemLink, bool>? value) => obj.SetValue(CanSetAssetLinkProperty, value);
    
    public static string Data => "AssetLink";
    public static Func<object?, AssetFileSystemLink>? GetData(AvaloniaObject obj) => GetGetAssetLink(obj);
    public static Action<AssetFileSystemLink>? GetSetData(AvaloniaObject obj) => GetSetAssetLink(obj);
    public static Func<AssetFileSystemLink, bool>? GetCanSetData(AvaloniaObject obj) => GetCanSetAssetLink(obj);
}
