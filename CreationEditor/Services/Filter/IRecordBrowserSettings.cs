using System.Reactive;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Filter;

public interface IRecordBrowserSettings {
    /// <summary>
    /// 
    /// </summary>
    string SearchTerm { get; set; }

    /// <summary>
    /// 
    /// </summary>
    ISearchFilter SearchFilter { get; }

    IModScopeProvider ModScopeProvider { get; }

    Func<IMajorRecordGetter, bool>? CustomFilter { get; set; }

    IObservable<Unit> SettingsChanged { get; }

    bool Filter(IMajorRecordGetter record);
    bool Filter(IMajorRecordIdentifier record);
}
