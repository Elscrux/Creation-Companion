namespace LeveledList.Model;
using Noggog;

public sealed class TierGroup {
    public string GroupIdentifier { get; }
    private readonly Lazy<(List<TierIdentifier> Items, IReadOnlyList<TierGroup> Groups)> _evaluateTiers;

    public TierGroup(string groupIdentifier, IEnumerable<TierIdentifier> tiers, char separator) {
        GroupIdentifier = groupIdentifier;
        var tiers1 = tiers;
        var separator1 = separator;

        _evaluateTiers = new Lazy<(List<TierIdentifier> Items, IReadOnlyList<TierGroup> Groups)>(() => {
            var items = new List<TierIdentifier>();
            var tiersPerGroup = new Dictionary<string, List<TierIdentifier>>();
            foreach (var tier in tiers1) {
                var span = tier.AsSpan();
                var indexOf = span.IndexOf(separator1);
                if (indexOf == -1) {
                    items.Add(tier);
                    continue;
                }

                var group = span[..indexOf];
                var rest = span[(indexOf + 1)..];
                tiersPerGroup.GetOrAdd(group.ToString(), _ => []).Add(rest.ToString());
            }
            
            var groups = tiersPerGroup
                .Select(x => new TierGroup(x.Key, x.Value, separator))
                .ToArray();

            return (items, groups);
        });
    }

    public List<TierIdentifier> GetItems() => _evaluateTiers.Value.Items;
    public IEnumerable<TierGroup> GetGroups() => _evaluateTiers.Value.Groups;
}
