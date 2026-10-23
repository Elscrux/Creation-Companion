using System.Text.RegularExpressions;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
using CreationEditor.Services.Integrations.Textures;
namespace ReleasePipeline.Services.Actions;

public enum TextureScaleFactor {
    Keep = 1,
    Half = 2,
    Quarter = 4,
    Eighth = 8,
    Sixteenth = 16,
}

/// <summary>
/// Config definition for a single texture processing rule: conditions select which textures the
/// rule applies to (first matching rule wins), operations define what is done with them. The
/// configured values of a rule are held separately in a
/// <see cref="ConfiguredValues{TextureRule}" /> item.
/// </summary>
public sealed class TextureRule : IConfigDefinition {
    // --- Conditions ---
    public static TextConfigOption SourcePathPatternOption { get; } = new(
        "Source Path Pattern",
        "Pattern is matched against the source texture's data relative path and supports * and ?",
        "*.dds"
    );

    public static EnumConfigOption SourceFormatOption { get; } = new(
        "Source Format",
        typeof(TextureFormat),
        "Required source format",
        isOptional: true
    );

    public static LongConfigOption SourceMinSizeOption { get; } = new(
        "Source Min Size (px)",
        "Minimum width and height in pixels, or unset for no minimum",
        isOptional: true
    );

    public static LongConfigOption SourceMaxSizeOption { get; } = new(
        "Source Max Size (px)",
        "Maximum width and height in pixels, or unset for no maximum",
        isOptional: true
    );

    // --- Operations ---
    public static EnumConfigOption TargetFormatOption { get; } = new(
        "Target Format",
        typeof(TextureFormat),
        "Format to convert the texture to, or Keep to leave it unchanged",
        isOptional: true
    );

    public static EnumConfigOption TargetScaleFactorOption { get; } = new(
        "Scale Factor",
        typeof(TextureScaleFactor),
        "Resize scale factor applied to width and height (1 = no resize)",
        TextureScaleFactor.Keep
    );

    public static LongConfigOption TargetMinResolutionOption { get; } = new(
        "Target Min Resolution (px)",
        "Lower bound for the resized dimensions in pixels; results are never scaled below this",
        128
    );

    public static LongConfigOption TargetMaxResolutionOption { get; } = new(
        "Target Max Resolution (px)",
        "Upper bound for the resized dimensions in pixels; results never exceed this, or unset for no cap",
        isOptional: true
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        SourcePathPatternOption,
        SourceFormatOption,
        SourceMinSizeOption,
        SourceMaxSizeOption,
        TargetFormatOption,
        TargetScaleFactorOption,
        TargetMinResolutionOption,
        TargetMaxResolutionOption,
    ];

    /// <summary>
    /// Whether this rule applies to the given texture.
    /// </summary>
    public bool Matches(ConfigValues configValues, TextureInfo info, string relativePath) {
        var pathPattern = configValues.Get(SourcePathPatternOption);
        if (!string.IsNullOrWhiteSpace(pathPattern) && !MatchesGlob(relativePath, pathPattern)) return false;

        var sourceFormat = configValues.TryGetStruct<TextureFormat>(SourceFormatOption);
        if (sourceFormat.HasValue && !info.FormatName.Equals(sourceFormat.ToString(), StringComparison.OrdinalIgnoreCase)) return false;

        var minSize = configValues.TryGetStruct(SourceMinSizeOption);
        if (info.Width < minSize || info.Height < minSize) return false;

        var maxSize = configValues.TryGetStruct(SourceMaxSizeOption);
        if (maxSize is > 0 && (info.Width > maxSize || info.Height > maxSize)) return false;

        return true;
    }

    /// <summary>
    /// Computes the resized dimensions for the given texture, or null if no resize is needed.
    /// </summary>
    public (uint Width, uint Height)? GetTargetSize(ConfigValues configValues, TextureInfo info) {
        var scaleFactor = configValues.Get<float>(TargetScaleFactorOption);

        var minResolution = configValues.Get(TargetMinResolutionOption);
        var maxResolution = configValues.TryGetStruct(TargetMaxResolutionOption);

        var width = ClampResolution((long) Math.Round(info.Width / scaleFactor), info.Width, minResolution, maxResolution);
        var height = ClampResolution((long) Math.Round(info.Height / scaleFactor), info.Height, minResolution, maxResolution);

        if (width == info.Width && height == info.Height) return null;

        return ((uint) width, (uint) height);
    }

    private static long ClampResolution(long value, long original, long minResolution, long? maxResolution) {
        if (minResolution > 0) value = Math.Max(value, minResolution);
        if (maxResolution is > 0) value = Math.Min(value, maxResolution.Value);

        // Never upscale beyond the original size through clamping
        return Math.Min(value, original);
    }

    /// <summary>Case-insensitive wildcard match supporting * and ?.</summary>
    private static bool MatchesGlob(string path, string pattern) {
        var regex = new Regex(
            "^" + Regex.Escape(pattern).Replace(@"\*\*", ".*").Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
            RegexOptions.IgnoreCase);
        return regex.IsMatch(path);
    }
}
