using WaterFlowPainter.Model;
namespace WaterFlowPainter.Services;

public enum FlowBrushMode {
    Direction,
    Speed,
    Scale,
}

/// <summary>
/// Applies brush strokes to a flowmap. Each stroke paints a circular area that blends the
/// affected pixels towards the brush's target values with a smooth falloff.
/// </summary>
public sealed class FlowmapPainter {
    /// <summary>
    /// Paints a single brush dab at the given flowmap position.
    /// </summary>
    /// <param name="flowmap">Flowmap to paint on</param>
    /// <param name="x">Brush center X in flowmap pixels</param>
    /// <param name="y">Brush center Y in flowmap pixels</param>
    /// <param name="mode">What the brush paints</param>
    /// <param name="radius">Brush radius in pixels</param>
    /// <param name="strength">Blend strength 0-1 at the brush center</param>
    /// <param name="direction">Target direction in radians (Direction mode)</param>
    /// <param name="speed">Target speed 0-1 (Speed mode)</param>
    /// <param name="scale">Target scale 0-1 (Scale mode)</param>
    public void Paint(
        FlowmapData flowmap,
        float x,
        float y,
        FlowBrushMode mode,
        float radius,
        float strength,
        float direction,
        float speed,
        float scale) {
        radius = MathF.Max(0.5f, radius);
        var minX = Math.Max(0, (int) MathF.Floor(x - radius));
        var maxX = Math.Min(FlowmapData.Size - 1, (int) MathF.Ceiling(x + radius));
        var minY = Math.Max(0, (int) MathF.Floor(y - radius));
        var maxY = Math.Min(FlowmapData.Size - 1, (int) MathF.Ceiling(y + radius));

        var targetX = MathF.Cos(direction);
        var targetY = MathF.Sin(direction);

        for (var py = minY; py <= maxY; py++) {
            for (var px = minX; px <= maxX; px++) {
                var distance = MathF.Sqrt((px + 0.5f - x) * (px + 0.5f - x) + (py + 0.5f - y) * (py + 0.5f - y));
                if (distance > radius) continue;

                // Smooth falloff from the brush center to its border
                var falloff = 1 - distance / radius;
                falloff *= falloff * (3 - 2 * falloff);
                var blend = strength * falloff;
                if (blend <= 0) continue;

                var index = FlowmapData.Index(px, py);
                switch (mode) {
                    case FlowBrushMode.Direction:
                        flowmap.Direction[index] = BlendDirection(flowmap.Direction[index], targetX, targetY, blend);
                        break;

                    case FlowBrushMode.Speed:
                        flowmap.Speed[index] = float.Lerp(flowmap.Speed[index], speed, blend);
                        break;

                    case FlowBrushMode.Scale:
                        flowmap.Scale[index] = float.Lerp(flowmap.Scale[index], scale, blend);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Blends a direction angle toward a target direction by blending the underlying unit vectors,
    /// which correctly handles the wrap-around at 0/360°.
    /// </summary>
    private static float BlendDirection(float currentAngle, float targetX, float targetY, float blend) {
        var currentX = MathF.Cos(currentAngle);
        var currentY = MathF.Sin(currentAngle);

        var x = float.Lerp(currentX, targetX, blend);
        var y = float.Lerp(currentY, targetY, blend);

        var length = MathF.Sqrt(x * x + y * y);
        if (length < 0.0001f) return currentAngle;

        var angle = MathF.Atan2(y / length, x / length);
        if (angle < 0) angle += MathF.Tau;
        return angle;
    }
}
