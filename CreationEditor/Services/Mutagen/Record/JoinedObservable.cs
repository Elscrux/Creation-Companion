using System.Reactive.Disposables;
using System.Reactive.Subjects;
namespace CreationEditor.Services.Mutagen.Record;

/// <summary>
/// An observable that takes a subject with an update action.
/// All subscribing objects will be called before and after the update action is executed whenever the subject emits an update. 
/// </summary>
/// <typeparam name="T">Type of the object that is observed</typeparam>
public sealed class JoinedObservable<T> {
    public Subject<UpdateAction<T>> Subject { get; }

    private readonly List<Func<T, Action<T>>> _actions = [];

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

    /// <summary>
    /// Register an update function that will be called before and after every update to the underlying observable.
    /// </summary>
    /// <param name="function">Function that takes the object pre-update
    /// and returns an action that should be called when the update is performed.</param>
    /// <returns>Disposable to unregister the update function</returns>
    public IDisposable Subscribe(Func<T, Action<T>> function) {
        _actions.Add(function);

        return Disposable.Create(() => _actions.Remove(function));
    }
}
