using Avalonia.Controls;
namespace CreationEditor.Avalonia;

public static class FlyoutExtension {
    extension(Flyout flyout) {
        public void ShowOnHover(Control trigger) {
            flyout.ShowMode = FlyoutShowMode.Transient;
            trigger.PointerEntered += (_, _) => flyout.ShowAt(trigger);
        }
    }
}
