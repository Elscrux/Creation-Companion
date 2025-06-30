using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Services.Mutagen.References.Record.Controller;

public interface IRecordDecorationController {
    IObservable<TDecoration> GetObservable<TDecoration>(IFormKeyGetter formKeyGetter);
    IEnumerable<object> GetAll(IFormKeyGetter formKeyGetter);
    TDecoration? Get<TDecoration>(IFormKeyGetter formKeyGetter) where TDecoration : class;
    void Update<TDecoration>(IFormKeyGetter formKeyGetter, TDecoration value) where TDecoration : class;
}
