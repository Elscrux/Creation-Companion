namespace CreationEditor.Avalonia;

public sealed class DisposableCounterLock {
    private readonly Action _freeAction;
    private readonly object _lock = new();
    private uint _counter;

    public DisposableCounterLock(Action freeAction) {
        _freeAction = freeAction;
    }
    
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

            if (_counter == 0) _freeAction();
        }
    }

    public bool IsLocked() {
        lock (_lock) {
            return _counter > 0;
        }
    }

    private sealed class CounterDisposable : IDisposable {
        private readonly DisposableCounterLock _counter;

        public CounterDisposable(DisposableCounterLock counter) {
            _counter = counter;
        }

        public void Dispose() => _counter.Unlock();
    }
}
