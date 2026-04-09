using CreationEditor.Services.DataSource;
namespace CreationEditor.Services.Asset;

public sealed class DataSourceLinkEqualityComparer : IEqualityComparer<IDataSourceLink> {
    public static readonly DataSourceLinkEqualityComparer Instance = new();

    public bool Equals(IDataSourceLink? x, IDataSourceLink? y) {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return x.DataSource.Equals(y.DataSource) && x.DataRelativePath.Equals(y.DataRelativePath);
    }

    public int GetHashCode(IDataSourceLink obj) {
        return HashCode.Combine(obj.DataSource, obj.DataRelativePath);
    }
}
