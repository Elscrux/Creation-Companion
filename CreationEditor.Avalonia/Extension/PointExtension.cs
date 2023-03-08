using Avalonia;
namespace CreationEditor.Avalonia;

public static class PointExtension {
    public static double Distance(this Point p1, Point p2) {
        return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }

    public static double Distance(this Point point, Point lineStart, Vector lineVector) {
        return Math.Abs(Vector.Cross(point - lineStart, lineVector)) / lineVector.Length;
    }
}
