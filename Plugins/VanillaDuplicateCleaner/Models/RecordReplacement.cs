using CreationEditor.Avalonia.Models.Selectables;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace VanillaDuplicateCleaner.Models;

public sealed class RecordReplacement : ReactiveObject, IReactiveSelectable {
    public RecordReplacement(IMajorRecordGetter modified, IMajorRecordGetter vanillaReplacement, string type) {
        Modified = modified;
        VanillaReplacement = vanillaReplacement;
        Type = type;
    }

    [Reactive] public bool IsSelected { get; set; }
    public IMajorRecordGetter Modified { get; }
    public IMajorRecordGetter VanillaReplacement { get; }
    public string Type { get; }
}
