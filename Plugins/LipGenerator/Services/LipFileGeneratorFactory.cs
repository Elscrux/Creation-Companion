using System.IO.Abstractions;
using CreationEditor.Services.Environment;
using Serilog;
namespace LipGenerator.Services;

public sealed class LipFileGeneratorFactory(
    ILogger logger,
    IFileSystem fileSystem,
    IEditorEnvironment editorEnvironment,
    Func<FaceFxWrapperArgs, FaceFxWrapper> createFaceFx,
    Func<LipGeneratorArgs, LipGeneratorWrapper> createLipGen,
    Func<LipFuzerArgs, LipFuzerWrapper> createFuzer,
    Func<XwmEncoderArgs, XwmEncoderWrapper> createXwmEncoder) {

    public LipFileGenerator Create(
        ILipGeneratorArgs lipGenArgs,
        IFuzGeneratorArgs fuzGenArgs,
        IAudioEncoderArgs? xwmEncoderArgs = null) {

        // Select lip generator based on args type
        ILipGenerator lipGenerator = lipGenArgs switch {
            FaceFxWrapperArgs args => createFaceFx(args),
            LipGeneratorArgs args => createLipGen(args),
            _ => throw new NotSupportedException($"Unsupported lip generator arguments type: {lipGenArgs.GetType().Name}")
        };

        // Create fuz generator
        IFuzGenerator fuzGenerator = fuzGenArgs switch {
            LipFuzerArgs args => createFuzer(args),
            _ => throw new NotSupportedException($"Unsupported fuz generator arguments type: {fuzGenArgs.GetType().Name}")
        };

        // Create encoder if args provided (null = skip encoding)
        IAudioEncoder? audioEncoder = xwmEncoderArgs switch {
            XwmEncoderArgs args => createXwmEncoder(args),
            null => null,
            _ => throw new NotSupportedException($"Unsupported audio encoder arguments type: {xwmEncoderArgs.GetType().Name}")
        };

        return new LipFileGenerator(logger, fileSystem, editorEnvironment, lipGenerator, fuzGenerator, audioEncoder);
    }
}
