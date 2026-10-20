using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
namespace CreationEditor.Avalonia;

public static class TreeDataGridDragExtension {
    private static readonly MethodInfo TryGetDragInfoMethod = typeof(TreeDataGrid)
            .GetMethod("TryGetDragInfo", BindingFlags.NonPublic | BindingFlags.Static)
     ?? throw new MissingMethodException(typeof(TreeDataGrid).FullName, "TryGetDragInfo");

    extension(TreeDataGridRowDragEventArgs rowDragEventArgs) {
        public bool TryGetDragInfo(out DragInfo dragInfo) {
            var args = new object?[] { rowDragEventArgs.Inner, null };
            var success = (bool?) TryGetDragInfoMethod.Invoke(null, args) == true;

            dragInfo = success
                ? (DragInfo) args[1]!
                : null!;

            return success;
        }
    }
}
