using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Record.Browser;

public interface IRecordBrowserSettings {
    [Reactive] public bool OnlyActive { get; set; }
    [Reactive] public ILinkCache LinkCache { get; set; }
    [Reactive] public BrowserScope Scope { get; set; }
    [Reactive] public string SearchTerm { get; set; }

    public bool Filter(IMajorRecordIdentifier record);
}