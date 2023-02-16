using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using DynamicData;
using DynamicData.Binding;
using Noggog;
namespace CreationEditor.Extension; 

public static class ObservableCollectionExtension {
    public static ReadOnlyObservableCollection<TTarget> SelectObservableCollection<TSource, TTarget>(
        this ObservableCollection<TSource> source,
        Expression<Func<TSource, TTarget>> selector,
        IDisposableDropoff disposable)
        where TSource : INotifyPropertyChanged {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return source
            .ToObservableChangeSet()
            .AutoRefresh(selector)
            .ToObservableCollection(selector.Compile(), disposable);
    }
}
