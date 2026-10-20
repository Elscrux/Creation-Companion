namespace CreationEditor.Resources.Comparer;

public sealed class ReferenceEqualsComparer<T> : IEqualityComparer<T> {
    public static readonly ReferenceEqualsComparer<T> Default = new();

    public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

    public int GetHashCode(T? obj) => obj?.GetHashCode() ?? 0;
}
