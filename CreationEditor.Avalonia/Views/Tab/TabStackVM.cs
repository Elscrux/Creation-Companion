using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Views.Tab; 

public class TabStackVM : ViewModel {
    public ObservableCollection<TabStackTab> Tabs { get; set; } = new();
    
    [Reactive] public Control? ActiveControl { get; set; }
    public ReactiveCommand<TabStackTab, Unit> Activate { get; }
    [Reactive] public float XWidth { get; set; } = 20;

    public TabStackVM() {
        Activate = ReactiveCommand.Create<TabStackTab>(tab => {
            if (ReferenceEquals(tab.Control, ActiveControl)) {
                ActiveControl = null;
                XWidth = 20;
            } else {
                ActiveControl = tab.Control;
                XWidth = 150;
            }
        });
    }
}
