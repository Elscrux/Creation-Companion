using DynamicData.Binding;
namespace CreationEditor.Avalonia.Models.GroupCollection;

/// <summary>
/// Items that have a common class are grouped together
/// </summary>
/// <param name="Class">Common class</param>
/// <param name="Items">Items with common class</param>
public sealed record GroupInstance(object? Class, IObservableCollection<object> Items) {
    private readonly Lock _modifyLock = new();

    /// <summary>
    /// Add items to the group instance
    /// </summary>
    /// <param name="items">Items to add</param>
    /// <param name="groups">Groups that the group instance currently groups by</param>
    /// <typeparam name="T">Type of items that are grouped</typeparam>
    public void Add<T>(IEnumerable<T> items, IReadOnlyList<Group<T>> groups) => Add(items, groups, 0);
    private void Add<T>(IEnumerable<T> items, IReadOnlyList<Group<T>> groups, int currentGroup) {
        lock (_modifyLock) {
            if (currentGroup < groups.Count) {
                var group = groups[currentGroup];
                var nextGroup = currentGroup + 1;

                if (Items.Count > 0) {
                    // If the collection already has items, add the new items to existing groups or add new groups
                    var groupedItems = items
                        .GroupBy(group.Selector);

                    foreach (var grouping in groupedItems) {
                        var @class = grouping.Key;

                        // If the group instance doesn't exist, create it and add it to the list
                        var groupInstance = GetClass(@class);
                        if (groupInstance is null) {
                            groupInstance = new GroupInstance(@class, new ObservableCollectionExtended<object>());
                            Items.Add(groupInstance);
                        }

                        groupInstance.Add(grouping, groups, nextGroup);
                    }
                } else {
                    // If the collection doesn't have any items, add all groups as new groups
                    var groupedItems = items
                        .GroupBy(group.Selector);

                    foreach (var grouping in groupedItems) {
                        var @class = grouping.Key;

                        // Create a new group instance and add it to the list
                        var groupInstance = new GroupInstance(@class, new ObservableCollectionExtended<object>());
                        Items.Add(groupInstance);

                        groupInstance.Add(grouping, groups, nextGroup);
                    }
                }
            } else {
                // If there are no more groups, add the items to the current group
                Items.AddRange(items.OfType<object>());
            }
        }
    }

    /// <summary>
    /// Remove items from the group instance
    /// </summary>
    /// <param name="items">Items to remove</param>
    /// <typeparam name="T">Type of items that are grouped</typeparam>
    /// <returns></returns>
    public bool Remove<T>(IEnumerable<T> items) {
        lock (_modifyLock) {
            var itemList = items.ToList();

            // We're a leaf group instance, remove the items from this group instance
            if (Items.FirstOrDefault() is T) {
                var anyRemoved = false;
                foreach (var item in itemList) {
                    if (item is null) continue;

                    if (Items.Remove(item)) {
                        anyRemoved = true;
                    }
                }
                return anyRemoved;
            }

            // Remove the items from the sub group instances
            var any = false;
            var groupInstances = Items.OfType<GroupInstance>().ToList();
            for (var i = groupInstances.Count - 1; i >= 0; i--) {
                var subGroupInstance = groupInstances[i];

                // Remove the items from the sub group instance
                if (!subGroupInstance.Remove(itemList)) continue;

                // Remove the sub group instance if it is empty
                if (subGroupInstance.Items.Count == 0) Items.RemoveAt(i);
                any = true;
            }

            return any;
        }
    }

    /// <summary>
    /// Group items by the specified group
    /// </summary>
    /// <param name="group">Group to group items by</param>
    /// <typeparam name="T">Type of items that are grouped</typeparam>
    public void GroupBy<T>(Group<T> group) {
        lock (_modifyLock) {
            var i = 0;
            while (i < Items.Count) {
                switch (Items[i]) {
                    case T t:
                        // Get the class of this item
                        var @class = group.Selector(t);

                        // Remove the item from the list
                        Items.RemoveAt(i);

                        // Check if we already have a group for this class
                        var groupInstance = GetClass(@class);

                        if (groupInstance is null) {
                            // Create new group instance
                            Items.Insert(i, new GroupInstance(@class, new ObservableCollectionExtended<object> { t }));
                            i++;
                        } else {
                            // Add to existing group instance
                            groupInstance.Items.Add(t);
                        }
                        break;
                    case GroupInstance subGroupInstance:
                        // Group the sub group instance
                        subGroupInstance.GroupBy(group);
                        i++;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Remove groups on the specified nesting level
    /// </summary>
    /// <param name="level">Level of group to remove</param>
    /// <typeparam name="T">Type of items that are grouped</typeparam>
    public void Ungroup<T>(int level) {
        lock (_modifyLock) {
            if (level == 0) {
                // We reached the level of our group
                var groupInstances = Items.OfType<GroupInstance>().ToList();

                // Replace the group instances with their items
                Items.LoadOptimized(groupInstances.SelectMany(x => x.Items));

                // Cleanup duplicate group instances
                Cleanup();
            } else {
                // Otherwise, keep going down
                var nextLevel = level - 1;
                foreach (var groupInstance in Items.OfType<GroupInstance>()) {
                    groupInstance.Ungroup<T>(nextLevel);
                }
            }
        }
    }

    /// <summary>
    /// Return all items in the group instance recursively
    /// </summary>
    /// <typeparam name="T">Type of items that are grouped</typeparam>
    /// <returns>Items in the group instance</returns>
    public IEnumerable<T> GetItems<T>() {
        foreach (var item in Items) {
            switch (item) {
                case T t:
                    yield return t;

                    break;
                case GroupInstance groupInstance:
                    foreach (var subItem in groupInstance.GetItems<T>()) {
                        yield return subItem;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Cleans up duplicate group instances
    /// </summary>
    private void Cleanup() {
        // Get all the group instances
        var groupInstances = Items.OfType<GroupInstance>().ToList();
        if (groupInstances.Count == 0) return;

        // Clear the groups instances
        Items.Clear();

        // Merge group instances with the same keys
        var mergedGroupInstances = groupInstances
            .GroupBy(x => x.Class)
            .Select(x => new GroupInstance(x.Key, new ObservableCollectionExtended<object>(x.SelectMany(g => g.Items))));

        // Add merged group instances
        Items.AddRange(mergedGroupInstances);
    }

    private GroupInstance? GetClass(object? @class) {
        return Items
            .OfType<GroupInstance>()
            .FirstOrDefault(x => x.Class?.Equals(@class) ?? false);
    }

    public override string ToString() {
        return Class?.ToString() ?? string.Empty;
    }
}
