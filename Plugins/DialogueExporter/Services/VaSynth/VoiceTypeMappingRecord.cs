using CsvHelper.Configuration.Attributes;
namespace DialogueExporter.Services.VaSynth;

public class VoiceTypeMappingRecord {
    [Name("VoiceType")] public string VoiceType { get; set; } = "";
    [Name("xVASynth model")] public string Model { get; set; } = "";
}
