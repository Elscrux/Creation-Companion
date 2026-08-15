using System.Diagnostics;
using CreationEditor;
using Serilog;
namespace LipGenerator.Services;

public sealed record XwmEncoderArgs(string XwmEncoderPath, int Bitrate) : IAudioEncoderArgs;

public sealed class XwmEncoderWrapper(
    ILogger logger,
    XwmEncoderArgs args) : IAudioEncoder {
    public string XwmEncoderPath { get; } = args.XwmEncoderPath;
    public int Bitrate { get; } = args.Bitrate;

    public static IReadOnlyList<int> SupportedBitrates { get; } = [20000, 32000, 48000, 64000, 96000, 160000, 192000];

    public string AudioExtension { get; set; } = "xwm";

    public void Encode(string pcmEncodedPath, string xwmPath) {
        var process = new Process {
            StartInfo = {
                FileName = XwmEncoderPath,
                Arguments = $"-b {Bitrate} \"{pcmEncodedPath}\" \"{xwmPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        if (process.ExitCode != 0) {
            logger.Here().Warning("XwmEncoder exited with code {ExitCode} for file {PcmEncodedPath}:\nOutput: {Output}\nError: {Error}",
                process.ExitCode,
                pcmEncodedPath,
                output,
                error);
        } else {
            logger.Here().Debug("Encoded xwm from {PcmEncodedPath} to {XwmPath} with bitrate {Bitrate}", pcmEncodedPath, xwmPath, Bitrate);
        }
    }
}
