using System.Reactive;
using System.Reactive.Linq;
namespace CreationEditor.WPF; 

public static class ObservableExtensions {
    
    // Will probably add to Noggog.CSharpExt sometime soon
    public static IObservable<T> RepublishLatestOnSignal<T>(this IObservable<T> source, IObservable<Unit> signal)
    {
        return source.Select(x =>
        {
            return signal
                .Select(_ => x)
                .StartWith(x);
        }).Switch();
    }
}
