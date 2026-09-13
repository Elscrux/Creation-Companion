using Mutagen.Bethesda.Skyrim;
using ReleasePipeline.Models.Check;
namespace ReleasePipeline.Services.Checks;

/// <summary>
/// Reports duplicate editor IDs within the mod, which can point at copy mistakes.
/// </summary>
public sealed class DuplicateEditorIdCheck : IReleaseCheck {
    public string Name => "Duplicate Editor IDs";
    public CheckSeverity Severity => CheckSeverity.Warning;

    public Task<CheckResult> RunAsync(ISkyrimModGetter mod, CancellationToken token) {
        var duplicates = mod.EnumerateMajorRecords()
            .Where(r => !string.IsNullOrEmpty(r.EditorID))
            .GroupBy(r => r.EditorID!)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicates.Count == 0) return Task.FromResult(CheckResult.Pass());

        return Task.FromResult(CheckResult.Warn(
            $"Found {duplicates.Count} editor ID(s) used more than once:{Environment.NewLine}" +
            string.Join(Environment.NewLine, duplicates.Select(g => $"{g.Key} ({g.Count()}x)"))));
    }
}
