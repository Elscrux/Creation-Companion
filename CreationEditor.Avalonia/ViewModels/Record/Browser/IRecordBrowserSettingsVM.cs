using System.Collections.ObjectModel;
using System.Reactive;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Avalonia.Models.Record.Browser;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public interface IRecordBrowserSettingsVM {
    [Reactive] public bool OnlyActive { get; set; }
    [Reactive] public ILinkCache LinkCache { get; set; }
    [Reactive] public BrowserScope Scope { get; set; }
    [Reactive] public string SearchTerm { get; set; }
    public ObservableCollection<ModItem> Mods { get; }
    
    public IObservable<Unit> SettingsChanged { get; }

    public bool Filter(IMajorRecordGetter record);
    public bool Filter(IMajorRecordIdentifier record);
}