using CsvHelper.Configuration.Attributes;
namespace DialogueExporter.Services.VaSynth;

public class VaSynthRecord {
    [Name("game_id")] public string GameId { get; set; } = "";
    [Name("voice_id")] public string VoiceId { get; set; } = "";
    [Name("text")] public string Text { get; set; } = "";
    [Name("vocoder")] public string Vocoder { get; set; } = "";
    [Name("out_path")] public string OutPath { get; set; } = "";
    [Name("pacing")] public string Pacing { get; set; } = "";
}
