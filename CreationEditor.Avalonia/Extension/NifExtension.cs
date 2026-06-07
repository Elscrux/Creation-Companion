using Avalonia.Media;
using NiflySharp.Structs;
using BSVertexData = nifly.BSVertexData;
using Color4 = NiflySharp.Structs.Color4;
namespace CreationEditor.Avalonia;

public static class NifExtension {
    extension(BSVertexData bsVertexData) {
        public Color ToRgbColor() => new(
            255,
            (byte) (bsVertexData.vert.x * 255),
            (byte) (bsVertexData.vert.y * 255),
            (byte) (bsVertexData.vert.z * 255));
    }

    extension(BSVertexDataSSE bsVertexData) {
        public Color ToRgbColor() => new(
            (byte) (bsVertexData.VertexColors.A * 255),
            (byte) (bsVertexData.VertexColors.R * 255),
            (byte) (bsVertexData.VertexColors.G * 255),
            (byte) (bsVertexData.VertexColors.B * 255));
    }

    extension(Color4 color4) {
        public Color ToRgbColor() => new(
            (byte) (color4.A * 255),
            (byte) (color4.R * 255),
            (byte) (color4.G * 255),
            (byte) (color4.B * 255));
        
        public ByteColor4 ToByteColor4() => new() {
            R = (byte) (color4.R * 255),
            G = (byte) (color4.G * 255),
            B = (byte) (color4.B * 255),
            A = (byte) (color4.A * 255),
        };
    }

    extension(Color color) {
        public Color4 ToColor4() => new(
            color.R / 255f,
            color.G / 255f,
            color.B / 255f,
            color.A / 255f);
    }

    extension(ByteColor4 color) {
        public Color4 ToColor4() => new(
            color.R / 255f,
            color.G / 255f,
            color.B / 255f,
            color.A / 255f);
    }
}
