namespace CreationEditor;

public sealed class DisposableCounterLock(Action freeAction) {
    private readonly Lock _lock = new();
    private uint _counter;

    /// <summary>
    /// Enters a waiting lock
    /// </summary>
    public IDisposable Lock() {
        lock (_lock) {
            _counter++;
            return new CounterDisposable(this);
        }
    }

    /// <summary>
    /// Enters a waiting lock
    /// </summary>
    private void Unlock() {
        lock (_lock) {
            _counter--;

            if (_counter == 0) freeAction();
        }
    }

    public bool IsLocked() {
        lock (_lock) {
            return _counter > 0;
        }
    }

    private sealed class CounterDisposable(DisposableCounterLock counter) : IDisposable {
        public void Dispose() => counter.Unlock();
    }
}
