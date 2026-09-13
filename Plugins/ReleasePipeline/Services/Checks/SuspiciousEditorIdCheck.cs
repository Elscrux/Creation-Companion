using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Services.Checks;

/// <summary>
/// Flags records whose editor ID looks unfinished or deletable: starting with "X" or
/// containing "delete" / "remove" / "delet".
/// </summary>
public sealed class SuspiciousEditorIdCheck : IReleaseCheck {
    private static readonly string[] DisallowedFragments = ["delete", "delet", "remove"];

    public string Name => "Suspicious Editor IDs";
    public CheckSeverity Severity => CheckSeverity.Warning;

    public Task<CheckResult> RunAsync(ISkyrimModGetter mod, CancellationToken token) {
        var matches = new List<string>();

        foreach (var record in mod.EnumerateMajorRecords()) {
            token.ThrowIfCancellationRequested();

            var editorId = record.EditorID;
            if (string.IsNullOrEmpty(editorId)) continue;

            if (editorId.StartsWith('X') || // XDelete style markers
                DisallowedFragments.Any(editorId.Contains)) {
                matches.Add(editorId);
            }
        }

        if (matches.Count == 0) return Task.FromResult(CheckResult.Pass());

        return Task.FromResult(CheckResult.Warn(
            $"Found {matches.Count} record(s) with suspicious editor IDs:{Environment.NewLine}{string.Join(Environment.NewLine, matches.Take(10))}"));
    }
}
