using System.Windows.Controls;
using Syncfusion.Windows.Tools.Controls;
namespace CreationEditor.GUI;

public static class DockingManagerExtension {
    public static TControl AddControl<TControl>(this DockingManager dockingManager, string header = "", DockSide dockSide = DockSide.Tabbed, DockState dockState = DockState.Document, double? size = null)
        where TControl : UserControl, new() {
        var control = new TControl();
        control.SetValue(DockingManager.HeaderProperty, header);
        control.SetValue(DockingManager.SideInDockedModeProperty, dockSide);
        control.SetValue(DockingManager.StateProperty, dockState);

        if (size != null) {
            switch (dockState) {
                case DockState.Dock when dockSide is DockSide.Left or DockSide.Right:
                    control.SetValue(DockingManager.DesiredWidthInDockedModeProperty, size);
                    break;
                case DockState.Dock when dockSide is DockSide.Bottom or DockSide.Top:
                    control.SetValue(DockingManager.DesiredHeightInDockedModeProperty, size);
                    break;
                case DockState.Float when dockSide is DockSide.Left or DockSide.Right:
                    control.SetValue(DockingManager.DesiredWidthInFloatingModeProperty, size);
                    break;
                case DockState.Float when dockSide is DockSide.Bottom or DockSide.Top:
                    control.SetValue(DockingManager.DesiredHeightInFloatingModeProperty, size);
                    break;
            }
        }
        
        dockingManager.Children.Add(control);

        return control;
    }
}
