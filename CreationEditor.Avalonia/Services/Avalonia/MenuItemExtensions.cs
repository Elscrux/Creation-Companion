using Avalonia.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

public static class MenuItemExtensions {
    public static MenuItem Name(this MenuItem menuItem, string name) {
        menuItem.Name = name;
        return menuItem;
    }
}
