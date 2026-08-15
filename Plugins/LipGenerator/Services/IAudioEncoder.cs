namespace LipGenerator.Services;

public interface IAudioEncoderArgs;

public interface IAudioEncoder {
    string AudioExtension { get; set; }

    void Encode(string pcmEncodedPath, string audioPath);
}
