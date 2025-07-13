using System.Reactive.Disposables;
using CreationEditor.Services.Mutagen.References.Cache;
namespace CreationEditor.Services.Mutagen.References;

public interface IReferenceUpdateTrigger<TSource, TSourceElement, TCache, TLink, TReference, TSubscriber>
    where TSource : notnull
    where TSourceElement : notnull
    where TCache : IReferenceCache<TCache, TLink, TReference>
    where TLink : notnull
    where TReference : notnull
    where TSubscriber : IReferenced {
    TReference ToReference(TSourceElement element);
    TSource? GetSourceFor(TSourceElement element);
    void SetupSubscriptions(
        ReferenceController<TSource, TSourceElement, TCache, TLink, TReference, TSubscriber> referenceController,
        CompositeDisposable disposables);
}
