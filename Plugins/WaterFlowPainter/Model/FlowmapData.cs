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

    public readonly WaterFlowDirection[] Direction = new WaterFlowDirection[PixelCount];

    /// <summary>Flow speed per pixel, normalized 0-1.</summary>
    public readonly float[] Speed = new float[PixelCount];

    /// <summary>Flow scale per pixel, normalized 0-1.</summary>
    public readonly float[] Scale = new float[PixelCount];

    public FlowmapData Clone() {
        var clone = new FlowmapData();
        Array.Copy(Direction, clone.Direction, PixelCount);
        Array.Copy(Speed, clone.Speed, PixelCount);
        Array.Copy(Scale, clone.Scale, PixelCount);
        return clone;
    }

    public void SetFrom(FlowmapData other) {
        Array.Copy(other.Direction, Direction, PixelCount);
        Array.Copy(other.Speed, Speed, PixelCount);
        Array.Copy(other.Scale, Scale, PixelCount);
    }

    public static int Index(int x, int y) => y * Size + x;

    /// <summary>
    /// Encodes the flow data into a 64x64 BGRA bitmap (unpremultiplied alpha).
    /// Flowmap row 0 is the south edge, while bitmap row 0 is the top, so the Y axis is flipped.
    /// </summary>
    public SKBitmap Encode() {
        var bitmap = new SKBitmap(Size, Size, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        var pixels = new byte[PixelCount * 4];
        for (var y = 0; y < Size; y++) {
            for (var x = 0; x < Size; x++) {
                var i = Index(x, y);
                var bitmapIndex = Index(x, Size - 1 - y) * 4;
                pixels[bitmapIndex + 0] = (byte) (Scale[i] * 255);
                pixels[bitmapIndex + 1] = (byte) (Direction[i].DirectionA * 255);
                pixels[bitmapIndex + 2] = (byte) (Direction[i].DirectionB * 255);
                pixels[bitmapIndex + 3] = (byte) (Speed[i] * 255);
            }
        }

        unsafe {
            fixed (byte* source = pixels) {
                Buffer.MemoryCopy(source, (void*) bitmap.GetPixels(), pixels.Length, pixels.Length);
            }
        }

        return bitmap;
    }

    /// <summary>
    /// Decodes a 64x64 BGRA bitmap (unpremultiplied alpha) into flow data.
    /// Bitmap row 0 is the top, while flowmap row 0 is the south edge, so the Y axis is flipped.
    /// </summary>
    public static FlowmapData Decode(SKBitmap bitmap) {
        var data = new FlowmapData();
        const int byteCount = PixelCount * 4;
        var pixels = new byte[byteCount];
        unsafe {
            fixed (byte* destination = pixels) {
                Buffer.MemoryCopy((void*) bitmap.GetPixels(), destination, byteCount, byteCount);
            }
        }

        for (var y = 0; y < Size; y++) {
            for (var x = 0; x < Size; x++) {
                var i = Index(x, y);
                var bitmapIndex = Index(x, Size - 1 - y) * 4;
                data.Scale[i] = pixels[bitmapIndex + 0] / 255f;
                data.Direction[i] = new WaterFlowDirection(pixels[bitmapIndex + 1] / 255f, pixels[bitmapIndex + 2] / 255f);
                data.Speed[i] = pixels[bitmapIndex + 3] / 255f;
            }
        }

        return data;
    }
}
