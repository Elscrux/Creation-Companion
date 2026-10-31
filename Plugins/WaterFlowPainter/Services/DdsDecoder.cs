using CreationEditor;
using Serilog;
using SkiaSharp;
namespace WaterFlowPainter.Services;

/// <summary>
/// Minimal in-process DDS decoder for uncompressed 32 bit textures. Supports the classic
/// uncompressed pixel format (RGB bitcount 32 with BGRA/BGRX masks) as well as the DX10
/// extension header with <c>DXGI_FORMAT_B8G8R8A8_UNORM</c> and <c>DXGI_FORMAT_R8G8B8A8_UNORM</c>,
/// which covers the <c>B8G8R8A8_UNORM</c> format Skyrim flowmaps are saved in.
/// Only the largest mip level of 2D textures is decoded.
/// </summary>
public sealed class DdsDecoder(ILogger logger) {
    private const uint DdsMagic = 0x20534444; // "DDS "
    private const uint DdpfFourcc = 0x4;
    private const uint DxgiFormatB8g8r8a8Unorm = 87;
    private const uint DxgiFormatR8g8b8a8Unorm = 28;
    private const int HeaderSize = 128;
    private const int Dx10HeaderSize = 20;

    /// <summary>Decoded DDS texture: raw BGRA bytes (unpremultiplied), row-major, top-down.</summary>
    public sealed record DecodedDds(int Width, int Height, byte[] BgraPixels);

    public SKBitmap? Decode(string filePath) {
        try {
            using var stream = File.OpenRead(filePath);
            return Decode(stream);
        } catch (Exception e) {
            logger.Here().Error(e, "Failed to read DDS file {Path}: {Message}", filePath, e.Message);
            return null;
        }
    }

    public SKBitmap? Decode(Stream stream) {
        var decoded = DecodeBytes(stream);
        if (decoded is null) return null;

        var bitmap = new SKBitmap(decoded.Width, decoded.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
        var address = bitmap.GetPixels();
        unsafe {
            fixed (byte* source = decoded.BgraPixels) {
                Buffer.MemoryCopy(source, (void*) address, decoded.BgraPixels.Length, decoded.BgraPixels.Length);
            }
        }

        return bitmap;
    }

    public DecodedDds? DecodeBytes(Stream stream) {
        using var reader = new BinaryReader(stream);
        if (stream.Length < HeaderSize) {
            logger.Here().Error("DDS file too small to contain a header");
            return null;
        }

        if (reader.ReadUInt32() != DdsMagic) {
            logger.Here().Error("Not a DDS file (invalid magic)");
            return null;
        }

        var headerSize = reader.ReadUInt32();
        if (headerSize < 124) {
            logger.Here().Error("Invalid DDS header size {Size}", headerSize);
            return null;
        }

        var flags = reader.ReadUInt32();
        var height = reader.ReadUInt32();
        var width = reader.ReadUInt32();
        var pitch = reader.ReadUInt32();
        _ = reader.ReadUInt32(); // depth
        _ = reader.ReadUInt32(); // mip map count

        // Reserved (11 * 4 bytes)
        stream.Seek(44, SeekOrigin.Current);

        // DDS_PIXELFORMAT
        var pixelFormatSize = reader.ReadUInt32();
        var pixelFlags = reader.ReadUInt32();
        var fourCc = reader.ReadUInt32();
        var rgbBitCount = reader.ReadUInt32();
        var rMask = reader.ReadUInt32();
        var gMask = reader.ReadUInt32();
        var bMask = reader.ReadUInt32();
        var aMask = reader.ReadUInt32();

        // Caps (4 * 4 bytes) + reserved2 (4 bytes)
        stream.Seek(20, SeekOrigin.Current);

        var hasDx10Header = (pixelFlags & DdpfFourcc) != 0 && fourCc == 0x3031584E; // "DX10"
        if (hasDx10Header) {
            if (stream.Length < HeaderSize + Dx10HeaderSize) {
                logger.Here().Error("DDS file too small to contain a DX10 header");
                return null;
            }

            var dxgiFormat = reader.ReadUInt32();
            _ = reader.ReadUInt32(); // resource dimension
            _ = reader.ReadUInt32(); // misc flags
            _ = reader.ReadUInt32(); // array size
            _ = reader.ReadUInt32(); // misc flags 2

            if (dxgiFormat is not (DxgiFormatB8g8r8a8Unorm or DxgiFormatR8g8b8a8Unorm)) {
                logger.Here().Error("Unsupported DX10 DDS format {Format}, only uncompressed B8G8R8A8/R8G8B8A8 are supported", dxgiFormat);
                return null;
            }
        } else if (rgbBitCount == 32) {
            // Classic uncompressed format, use the channel masks
        } else if (rgbBitCount == 0 && fourCc == 0 && pitch == width * 4) {
            // Some tools write an empty pixel format block; assume 32 bit BGRA when the pitch matches
            rgbBitCount = 32;
        } else {
            logger.Here().Error("Unsupported DDS format (flags {Flags}, bit count {BitCount}, fourCC {FourCC}), only uncompressed 32 bit textures are supported",
                pixelFlags,
                rgbBitCount,
                fourCc);
            return null;
        }

        var isBgra = hasDx10Header
         || (rMask == 0x00FF0000 && gMask == 0x0000FF00 && bMask == 0x000000FF)
         || (rMask == 0 && gMask == 0 && bMask == 0 && aMask == 0);

        var pixelCount = (int) (width * height);
        var pixels = new byte[pixelCount * 4];
        if (stream.Read(pixels, 0, pixels.Length) != pixels.Length) {
            logger.Here().Error("DDS file ended before all pixel data could be read");
            return null;
        }

        if (!isBgra) {
            // R8G8B8A8: swap R and B channels to get BGRA
            for (var i = 0; i < pixels.Length; i += 4) {
                (pixels[i], pixels[i + 2]) = (pixels[i + 2], pixels[i]);
            }
        }

        return new DecodedDds((int) width, (int) height, pixels);
    }
}
