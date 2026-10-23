using Mutagen.Bethesda.Analyzers;
using Mutagen.Bethesda.Analyzers.SDK.Topics;
namespace ReleasePipeline.Services.Checks;

/// <summary>
/// Helper for creating custom analyzer topic definitions for release checks.
/// </summary>
public static class CheckTopic {
    public static TopicDefinition Custom(string title, Severity severity) {
        var id = (ushort) Random.Shared.Next(ushort.MaxValue);
        return new TopicDefinition(
            new TopicId("CC", id),
            title,
            severity,
            informationUri: new Uri($"{Constants.AnalyzersDiscussionUrl.TrimEnd('/')}/{id.ToString()}"));
    }
}
