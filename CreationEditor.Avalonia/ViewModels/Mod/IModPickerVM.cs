using System.Collections.ObjectModel;
using CreationEditor.Avalonia.Models.Mod;
namespace CreationEditor.Avalonia.ViewModels.Mod;

public interface IModPickerVM {
    ReadOnlyObservableCollection<LoadOrderModItem> Mods { get; }
}
