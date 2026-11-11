namespace WaterFlowPainter.Model;

public readonly struct WaterFlowDirection(float a, float b) {
    public readonly float DirectionA = a;
    public readonly float DirectionB = b;

    public static WaterFlowDirection FromAngleRadians(float radians) {
        var x = Math.Cos(radians);
        var y = Math.Sin(radians);
        var m = Math.Max(Math.Abs(x), Math.Abs(y));

        var a = (float) (1 + y / m) / 2;
        var b = (float) (1 + x / m) / 2;

        return new WaterFlowDirection(a, b);
    }

    public static WaterFlowDirection FromAngleDegrees(float degrees) => FromAngleRadians(degrees / 180 * (float) Math.PI);

    public (float X, float Y) ToVector() {
        var a = DirectionA * 2 - 1;
        var b = DirectionB * 2 - 1;

        var m = Math.Max(Math.Abs(a), Math.Abs(b));
        if (m == 0) return (0, 0);

        var y = a * m;
        var x = b * m;

        return (x, y);
    }
}
