using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

public interface IRecordDecorationController {
    void Register<TDecoration>() where TDecoration : class;

    IReadOnlyDictionary<IFormLinkGetter, TDecoration> GetAllDecorations<TDecoration>() where TDecoration : class ;
    IObservable<TDecoration> GetObservable<TDecoration>(IFormLinkGetter formLink);
    IEnumerable<object> GetAll(IFormLinkGetter formLink);
    TDecoration? Get<TDecoration>(IFormLinkGetter formLink) where TDecoration : class;
    void Update<TDecoration>(IFormLinkGetter formLink, TDecoration value) where TDecoration : class;
    void Delete<TDecoration>(IMajorRecordGetter record) where TDecoration : class;
}
