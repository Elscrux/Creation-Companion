using System.IO.Enumeration;
using CreationEditor.Services.Config;
using CreationEditor.Services.Config.Options;
namespace ReleasePipeline.Models;

/// <summary>
/// Config definition for a single audio encoding rule: the first rule whose file filter matches a
/// file determines the bitrate used to encode it. The configured values of a rule are held
/// separately in a <see cref="ConfiguredValues{AudioEncodingRule}" /> item.
/// </summary>
public sealed class AudioEncodingRule : IConfigDefinition {
    public static TextConfigOption FileFilterOption { get; } = new(
        "File Filter",
        "Wildcard filter (supports * and ?) matched against the file's path and file name, or empty to match any file",
        @"sound\*"
    );

    public static BoolConfigOption DontEncodeOption { get; } = new(
        "Don't Encode",
        "Files that match this rule will not be encoded, and kept in a decoded format, including previously encoded files which will be decoded"
    );

    public static LongConfigOption BitrateOption { get; } = new(
        "Bitrate",
        "Target bitrate in kbps used to encode matching files",
        96000
    );

    public static BoolConfigOption ReEncodeOption { get; } = new(
        "Re-encode",
        "Files that are already encoded in the target format will be re-encoded with the specified bitrate, otherwise they files will be skipped and left at their current bitrate",
        true
    );

    public static BoolConfigOption ForceMonoOption { get; } = new(
        "Force Mono",
        "Files will be forced to use a single audio channel, files with multiple channels will be downmixed to mono",
        true
    );

    public static BoolConfigOption PackageFuzOption { get; } = new(
        "Fuze Lips",
        "Package lip and audio files into fuz files",
        true
    );

    public IReadOnlyList<IConfigOption> Options { get; } = [
        FileFilterOption,
        DontEncodeOption,
        BitrateOption,
        ReEncodeOption,
        ForceMonoOption,
        PackageFuzOption,
    ];

    public bool Matches(ConfigValues configValues, string path) {
        var fileFilter = configValues.TryGet(FileFilterOption);
        if (string.IsNullOrWhiteSpace(fileFilter)) return true;

        fileFilter = fileFilter.Replace(@"\", @"\\");
        fileFilter = "*" + fileFilter;
        return FileSystemName.MatchesSimpleExpression(fileFilter, path);
    }
}
