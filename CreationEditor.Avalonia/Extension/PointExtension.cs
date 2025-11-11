using Avalonia;
namespace CreationEditor.Avalonia;

public static class PointExtension {
    extension(Point p1) {
        public double Distance(Point p2) {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        public double Distance(Point lineStart, Vector lineVector) {
            return Math.Abs(Vector.Cross(p1 - lineStart, lineVector)) / lineVector.Length;
        }
    }
}
