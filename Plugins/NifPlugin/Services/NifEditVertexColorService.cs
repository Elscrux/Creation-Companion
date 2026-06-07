using Avalonia.Media;
using CreationEditor.Avalonia;
using NiflySharp;
using NiflySharp.Blocks;
using NiflySharp.Structs;
namespace NifPlugin.Services;

public sealed class NifEditVertexColorService {
    public bool HasVertexColor(INiObject block) => block switch {
        NiTriShapeData niTriShapeData => niTriShapeData.HasVertexColors,
        NiSkinPartition skinPartition => skinPartition.VertexDesc.ColorOffset > 0,
        BSTriShape bsTriShape => bsTriShape.HasVertexColors,
        _ => false
    };

    private static void ReplaceVertexColors(INiObject block, Func<Color4, Color4> selectorColor) {
        switch (block) {
            case NiTriShapeData { VertexColors: not null } niTriShapeData:
                niTriShapeData.VertexColors = niTriShapeData.VertexColors.Select(selectorColor).ToList();
                break;
            case NiSkinPartition skinPartition:
                skinPartition.SetVertexData(TransformVertexData(skinPartition.VertexData));
                break;
            case BSTriShape bsTriShape:
                bsTriShape.SetVertexDataSSE(TransformVertexData(bsTriShape.VertexDataSSE));
                break;
        }

        List<BSVertexDataSSE> TransformVertexData(IEnumerable<BSVertexDataSSE> vertexData) => vertexData
            .Select(vertexColor => {
                var oldColor = vertexColor.VertexColors.ToColor4();
                var newColor = selectorColor(oldColor);
                vertexColor.VertexColors = newColor.ToByteColor4();
                return vertexColor;
            })
            .ToList();
    }

    public void VertexColorHueShift(INiObject block, double hueShift) {
        ReplaceVertexColors(block,
            color => {
                var rgbColor = color.ToRgbColor();
                var hslColor = rgbColor.ToHsl();

                var newHslColor = new HslColor(hslColor.A, (hslColor.H + hueShift) % 360, hslColor.S, hslColor.L);
                var newRgbColor = newHslColor.ToRgb();

                return newRgbColor.ToColor4();
            });
    }

    public void VertexColorHueShift(INiObject block, uint blockId, double hueShift) {
        ReplaceVertexColors(block,
            color => {
                var rgbColor = color.ToRgbColor();
                var hslColor = rgbColor.ToHsl();

                var newHslColor = new HslColor(hslColor.A, (hslColor.H + hueShift) % 360, hslColor.S, hslColor.L);
                var newRgbColor = newHslColor.ToRgb();

                return newRgbColor.ToColor4();
            });
    }

    public void VertexColorSaturationBoost(INiObject block, double saturationShift) {
        ReplaceVertexColors(block,
            color => {
                var rgbColor = color.ToRgbColor();
                var hslColor = rgbColor.ToHsl();

                var newHslColor = new HslColor(hslColor.A, hslColor.H, Math.Clamp(hslColor.S + saturationShift / 255, 0, 1), hslColor.L);
                var newRgbColor = newHslColor.ToRgb();

                return newRgbColor.ToColor4();
            });
    }

    public void VertexColorSaturationBoost(INiObject block, uint blockId, double saturationShift) {
        ReplaceVertexColors(block,
            color => {
                var rgbColor = color.ToRgbColor();
                var hslColor = rgbColor.ToHsl();

                var newHslColor = new HslColor(hslColor.A, hslColor.H, Math.Clamp(hslColor.S + saturationShift / 255, 0, 1), hslColor.L);
                var newRgbColor = newHslColor.ToRgb();

                return newRgbColor.ToColor4();
            });
    }

    public void VertexColorLightness(INiObject block, double lightnessShift) {
        ReplaceVertexColors(block,
            color => {
                var rgbColor = color.ToRgbColor();
                var hslColor = rgbColor.ToHsl();

                var newHslColor = new HslColor(hslColor.A, hslColor.H, hslColor.S, Math.Clamp(hslColor.L + lightnessShift / 255, 0, 1));
                var newRgbColor = newHslColor.ToRgb();

                return newRgbColor.ToColor4();
            });
    }

    public void VertexColorLightness(INiObject block, uint blockId, double lightnessShift) {
        ReplaceVertexColors(block,
            color => {
                var rgbColor = color.ToRgbColor();
                var hslColor = rgbColor.ToHsl();

                var newHslColor = new HslColor(hslColor.A, hslColor.H, hslColor.S, Math.Clamp(hslColor.L + lightnessShift / 255, 0, 1));
                var newRgbColor = newHslColor.ToRgb();

                return newRgbColor.ToColor4();
            });
    }
}
