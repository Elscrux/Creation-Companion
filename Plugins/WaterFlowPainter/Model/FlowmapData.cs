using SkiaSharp;
namespace WaterFlowPainter.Model;

/// <summary>
/// Decoded flowmap of a single cell. A flowmap is a 64x64 texture where each pixel encodes
/// the water flow at that position: alpha = speed, red = direction (angle 0-255 → 0-360°),
/// green = scale. Blue is unused.
/// </summary>
public sealed class FlowmapData {
    public const int Size = 64;
    public const int PixelCount = Size * Size;

    /// <summary>Flow direction per pixel in radians, measured from +X (east) towards +Y (north).</summary>
    public readonly float[] Direction = new float[PixelCount];

    /// <summary>Flow speed per pixel, normalized 0-1.</summary>
    public readonly float[] Speed = new float[PixelCount];

    /// <summary>Flow scale per pixel, normalized 0-1.</summary>
    public readonly float[] Scale = new float[PixelCount];

    /// <summary>Raw blue channel per pixel. Unused by the game, but preserved to not corrupt existing flowmaps.</summary>
    public readonly byte[] Blue = new byte[PixelCount];

    public FlowmapData() {
        Array.Fill(Blue, (byte) 255);
    }

    public FlowmapData Clone() {
        var clone = new FlowmapData();
        Array.Copy(Direction, clone.Direction, PixelCount);
        Array.Copy(Speed, clone.Speed, PixelCount);
        Array.Copy(Scale, clone.Scale, PixelCount);
        Array.Copy(Blue, clone.Blue, PixelCount);
        return clone;
    }

    public void SetFrom(FlowmapData other) {
        Array.Copy(other.Direction, Direction, PixelCount);
        Array.Copy(other.Speed, Speed, PixelCount);
        Array.Copy(other.Scale, Scale, PixelCount);
        Array.Copy(other.Blue, Blue, PixelCount);
    }

    public static int Index(int x, int y) => y * Size + x;

    /// <summary>Encodes the flow data into a 64x64 BGRA bitmap (unpremultiplied alpha).</summary>
    public SKBitmap Encode() {
        var bitmap = new SKBitmap(Size, Size, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        var pixels = new byte[PixelCount * 4];
        for (var i = 0; i < PixelCount; i++) {
            pixels[i * 4 + 0] = Blue[i];
            pixels[i * 4 + 1] = (byte) (Scale[i] * 255 + 0.5f);
            pixels[i * 4 + 2] = (byte) (Direction[i] / MathF.Tau * 255 + 0.5f);
            pixels[i * 4 + 3] = (byte) (Speed[i] * 255 + 0.5f);
        }

        unsafe {
            fixed (byte* source = pixels) {
                Buffer.MemoryCopy(source, (void*) bitmap.GetPixels(), pixels.Length, pixels.Length);
            }
        }

        return bitmap;
    }

    /// <summary>Decodes a 64x64 BGRA bitmap (unpremultiplied alpha) into flow data.</summary>
    public static FlowmapData Decode(SKBitmap bitmap) {
        var data = new FlowmapData();
        const int byteCount = PixelCount * 4;
        var pixels = new byte[byteCount];
        unsafe {
            fixed (byte* destination = pixels) {
                Buffer.MemoryCopy((void*) bitmap.GetPixels(), destination, byteCount, byteCount);
            }
        }

        for (var i = 0; i < PixelCount; i++) {
            data.Blue[i] = pixels[i * 4 + 0];
            data.Scale[i] = pixels[i * 4 + 1] / 255f;
            data.Direction[i] = pixels[i * 4 + 2] / 255f * MathF.Tau;
            data.Speed[i] = pixels[i * 4 + 3] / 255f;
        }

        return data;
    }
}
