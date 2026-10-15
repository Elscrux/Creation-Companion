using Mutagen.Bethesda.Plugins;
namespace CreationEditor.Services.Environment;

public enum LoadOrderIssueType {
    /// <summary>A mod lists itself as one of its masters.</summary>
    SelfMaster,

    /// <summary>Mods reference each other as masters, so no valid load order exists for them.</summary>
    CyclicDependency,
}

/// <summary>
/// A problem detected while sorting the load order, e.g. a mod listing itself as master
/// or mods with cyclic master references.
/// </summary>
public sealed record LoadOrderIssue(LoadOrderIssueType Type, IReadOnlyList<ModKey> Mods) {
    public override string ToString() => $"{Type}: {string.Join(" -> ", Mods.Select(x => x.FileName))}";
}

public sealed record LoadOrderSortResult(IReadOnlyList<ModKey> Order, IReadOnlyList<LoadOrderIssue> Issues);

public sealed class LoadOrderSorter : ILoadOrderSorter {
    public LoadOrderSortResult Sort(IReadOnlyList<ModKey> modKeys, Func<ModKey, IEnumerable<ModKey>> getDirectMasters) {
        var keys = modKeys.Distinct().ToList();
        var keyIndices = keys.Select((key, index) => (key, index)).ToDictionary(x => x.key, x => x.index);
        var keySet = keyIndices.Keys.ToHashSet();

        var directMasters = keys.ToDictionary(
            key => key,
            key => getDirectMasters(key).Where(keySet.Contains).ToHashSet());

        var issues = new List<LoadOrderIssue>();

        // A mod listing itself as master can never be ordered correctly - drop the self reference and report it
        foreach (var key in keys) {
            if (!directMasters[key].Remove(key)) continue;

            issues.Add(new LoadOrderIssue(LoadOrderIssueType.SelfMaster, [key]));
        }

        // Topological sort: among all mods whose masters are already placed, master plugins come first, then mods keep their relative order
        var remainingMasters = directMasters.ToDictionary(x => x.Key, x => x.Value.Count);
        var dependents = keys.ToDictionary(key => key, _ => new List<ModKey>());
        foreach (var (key, masters) in directMasters) {
            foreach (var master in masters) {
                dependents[master].Add(key);
            }
        }

        var order = new List<ModKey>(keys.Count);
        while (order.Count < keys.Count) {
            // Select the next mod which has no remaining masters, preferring master plugins and then keeping the original order
            var next = keys
                .Where(key => remainingMasters[key] == 0)
                .OrderByDescending(key => key.Type == ModType.Master ? 1 : 0)
                .ThenBy(key => keyIndices[key])
                .FirstOrDefault();
            if (next.IsNull) break;

            order.Add(next);
            remainingMasters[next] = -1;
            foreach (var dependent in dependents[next]) {
                remainingMasters[dependent]--;
            }
        }

        // Mods that could not be placed are part of a cyclic master dependency
        var cyclic = keys.Where(key => remainingMasters[key] > 0).ToList();
        if (cyclic.Count > 0) {
            issues.Add(new LoadOrderIssue(LoadOrderIssueType.CyclicDependency, cyclic));
            order.AddRange(cyclic);
        }

        return new LoadOrderSortResult(order, issues);
    }
}
