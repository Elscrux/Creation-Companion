using System.ComponentModel;
using Noggog;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Selectables;

public interface IReactiveSelectable : ISelected, INotifyPropertyChanged {
    [Reactive] new bool IsSelected { get; set; }
}
