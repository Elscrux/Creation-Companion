using DynamicData.Binding;
namespace CreationEditor.Avalonia.Views;

public interface ISplashScreenVM {
    IObservableCollection<string> Messages { get; }
    int Counter { get; set; }
    int Total { get; set; }
}
