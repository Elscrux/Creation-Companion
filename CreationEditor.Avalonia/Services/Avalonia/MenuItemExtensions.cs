using Avalonia.Controls;
namespace CreationEditor.Avalonia.Services.Avalonia;

public static class MenuItemExtensions {
    extension(MenuItem menuItem) {
        public MenuItem Name(string name) {
            menuItem.Name = name;
            return menuItem;
        }
    }
}
