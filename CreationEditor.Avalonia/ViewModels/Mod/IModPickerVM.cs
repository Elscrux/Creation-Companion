using System.Collections.ObjectModel;
using CreationEditor.Avalonia.Models.Mod;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public interface IModPickerVM {
    ReadOnlyObservableCollection<OrderedModItem> Mods { get; }
    Func<OrderedModItem, bool>? Filter { get; set; }
}
