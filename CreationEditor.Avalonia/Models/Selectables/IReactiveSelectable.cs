using System.ComponentModel;
using Noggog;
namespace CreationEditor.Avalonia.Models.Selectables;

public interface IReactiveSelectable : ISelected, INotifyPropertyChanged {
    new bool IsSelected { get; set; }
}
