using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

public interface IRecordDecorationController {
    void Register<TDecoration>() where TDecoration : class;

    IReadOnlyDictionary<FormKey, TDecoration> GetAllDecorations<TDecoration>() where TDecoration : class ;
    IObservable<TDecoration> GetObservable<TDecoration>(IFormKeyGetter formKeyGetter);
    IEnumerable<object> GetAll(IFormKeyGetter formKeyGetter);
    TDecoration? Get<TDecoration>(IFormKeyGetter formKeyGetter) where TDecoration : class;
    void Update<TDecoration>(IFormKeyGetter formKeyGetter, TDecoration value) where TDecoration : class;
    void Delete<TDecoration>(IMajorRecordGetter record) where TDecoration : class;
}
