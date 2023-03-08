using System.ComponentModel;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Selectables;

public interface IReactiveSelectable : INotifyPropertyChanged {
    [Reactive] bool IsSelected { get; set; }
}
