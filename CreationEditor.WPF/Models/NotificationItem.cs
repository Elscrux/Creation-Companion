using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.WPF.Models;

public class NotificationItem : ReactiveObject {
    [Reactive] public string LoadText { get; set; }
    [Reactive] public float LoadProgress { get; set; }
    public Guid ID { get; }
    
    private readonly ObservableAsPropertyHelper<bool> _anyProgress;
    public bool AnyProgress => _anyProgress.Value;
    
    public NotificationItem(Guid id, string loadText, float loadProgress) {
        LoadText = loadText;
        LoadProgress = loadProgress;
        ID = id;

        _anyProgress = this
            .WhenAnyValue(x => x.LoadProgress)
            .Select(progress => progress != 0)
            .ToProperty(this, x => x.AnyProgress);
    }
    
}
