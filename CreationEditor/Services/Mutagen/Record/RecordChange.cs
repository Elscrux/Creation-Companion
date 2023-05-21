using System.Reactive.Disposables;
using System.Reactive.Subjects;
namespace CreationEditor.Services.Mutagen.Record;

public sealed record UpdateAction<T>(T Item, Action UpdateItem);

public sealed class JoinedObservable<T> {
    public Subject<UpdateAction<T>> Subject { get; }

    private readonly List<Func<T, Action<T>>> _actions = new();

    public JoinedObservable(Subject<UpdateAction<T>> subject) {
        Subject = subject;

        Subject.Subscribe(updateAction => {
            // Run all pre update actions and gather post update actions
            var postUpdateActions = _actions.Select(func => func(updateAction.Item)).ToArray();

            updateAction.UpdateItem();

            // Run all post update actions
            foreach (var postUpdateAction in postUpdateActions) {
                postUpdateAction(updateAction.Item);
            }
        });
    }

    public IDisposable Subscribe(Func<T, Action<T>> action) {
        _actions.Add(action);

        return Disposable.Create(() => _actions.Remove(action));
    }
}
