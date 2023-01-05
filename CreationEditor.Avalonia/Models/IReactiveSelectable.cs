using System.ComponentModel;
using Noggog;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia; 

public interface IReactiveSelectable : ISelected, INotifyPropertyChanged {
    [Reactive] new bool IsSelected { get; set; }
}
