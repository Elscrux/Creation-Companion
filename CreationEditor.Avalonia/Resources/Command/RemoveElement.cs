using System.Collections;
namespace CreationEditor.Avalonia.Command;

public class RemoveElement<T> : ListCommand<T> where T : new() {
    public override void Execute(object? parameter) {
        if (List == null) return;
        if (parameter is not IList removeList) return;

        foreach (var t in removeList.OfType<T>().ToList()) {
            List.Remove(t);
        }
    }
}
