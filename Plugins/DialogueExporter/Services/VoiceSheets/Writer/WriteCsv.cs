using DialogueExporter.Cli.Args;
namespace DialogueExporter.Services.VoiceSheets.Writer;

public sealed class WriteCsv(CommandLineEntryPointVoiceSheets args) {
    public void Write(List<ExportLine> lines) {
        foreach (var voiceTypeGrouping in lines.GroupBy(l => l.VoiceType)) {
            var voiceType = voiceTypeGrouping.Key;
            var writer = new StreamWriter(Path.Combine(args.OutputDirectory, $"{voiceType}.csv"));
            writer.WriteLine("""
                "You are speaking as","Context","Line to speak","Acting Note","Filename","Filecutting note"
                """);

            var speakerGrouping = voiceTypeGrouping.GroupBy(x => x.Speaker).ToArray();
            for (var i = 0; i < speakerGrouping.Length; i++) {
                if (i > 0) writer.WriteLine("");

                var speaker = speakerGrouping[i];
                var speakerArray = speaker.OrderBy(x => x.Quest.EditorID ?? x.Quest.Name?.String ?? x.Quest.FormKey.ToString()).ToArray();
                var questGrouping = speakerArray.GroupBy(x => x.Quest.EditorID ?? x.Quest.Name?.String ?? x.Quest.FormKey.ToString())
                    .ToArray();
                for (var j = 0; j < questGrouping.Length; j++) {
                    if (j > 0) writer.WriteLine("");

                    var quest = questGrouping[j];
                    var questArray = quest.OrderBy(x => x.Branch?.EditorID ?? x.Branch?.FormKey.ToString()).ToArray();
                    var branchGrouping = questArray.GroupBy(x => x.Branch?.EditorID ?? x.Branch?.FormKey.ToString()).ToArray();
                    for (var k = 0; k < branchGrouping.Length; k++) {
                        if (k > 0) writer.WriteLine("");

                        var branch = branchGrouping[k];
                        var branchArray = branch.OrderBy(x => x.Topic.EditorID ?? x.Topic.FormKey.ToString()).ToArray();
                        var topicGrouping = branchArray.GroupBy(x => x.Topic.EditorID ?? x.Topic.FormKey.ToString()).ToArray();
                        for (var l = 0; l < topicGrouping.Length; l++) {
                            if (l > 0) writer.WriteLine("");

                            var topic = topicGrouping[l];
                            var topicArray = topic.OrderByDescending(x => x.Responses.EditorID ?? x.Responses.FormKey.ToString()).ToArray();
                            var responsesGrouping = topicArray.GroupBy(x => x.Responses.EditorID ?? x.Topic.FormKey.ToString()).ToArray();
                            for (var m = 0; m < responsesGrouping.Length; m++) {
                                if (m > 0) writer.WriteLine("");

                                var responses = responsesGrouping[m];
                                var responsesArray = responses.ToArray();
                                foreach (var response in responsesArray.GroupBy(x => x.Response)) {
                                    var responseArray = response.OrderBy(x => x.Path).ToArray();
                                    foreach (var line in responseArray) {
                                        writer.WriteLine($"""
                                            "{line.Speaker}","{line.Context}","{line.Line}","{line.VaNote}","{Path.GetFileNameWithoutExtension(line.Path)}",""
                                            """);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            writer.Dispose();
        }
    }
}
