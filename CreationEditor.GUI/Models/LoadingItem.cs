using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.GUI.Models; 

public class LoadingItem : ReactiveObject {
    [Reactive] public string LoadText { get; set; }
    [Reactive] public float LoadProgress { get; set; }
    
    private readonly ObservableAsPropertyHelper<bool> _anyProgress;
    public bool AnyProgress => _anyProgress.Value;
    
    public LoadingItem(string loadText, float loadProgress) {
        LoadText = loadText;
        LoadProgress = loadProgress;

        _anyProgress = this
            .WhenAnyValue(x => x.LoadProgress)
            .Select(progress => progress != 0)
            .ToProperty(this, x => x.AnyProgress);
    }
    
}
