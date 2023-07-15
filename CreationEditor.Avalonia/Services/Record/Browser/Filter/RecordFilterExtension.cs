using CreationEditor.Avalonia.Models.Record.Browser;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public static class RecordFilterExtension {
    public static IEnumerable<RecordFilterListing> GetRecursiveListings<T>(this IEnumerable<T> elements,
        Func<T, IEnumerable<string>> stringSelector,
        params char[] separator) {

        var root = new RecordFilterListing(string.Empty, _ => true);

        foreach (var element in elements) {
            foreach (var selectedString in stringSelector(element)) {
                var accumulatedPath = string.Empty;
                var currentRoot = root;

                var directories = selectedString.Split(separator);
                for (var i = 0; i < directories.Length; i++) {
                    var directory = directories[i];
                    if (directory.IsNullOrEmpty()) break;

                    accumulatedPath += i == directories.Length - 1 ? directory : directory + separator;

                    var listing = currentRoot.RecordFilters.FirstOrDefault(x => string.Equals(x.DisplayName, directory, StringComparison.OrdinalIgnoreCase));
                    if (listing is null) {
                        var path = accumulatedPath;
                        listing = new RecordFilterListing(
                            directory,
                            record => {
                                if (record is not T t) return false;

                                return stringSelector(t)
                                    .Any(selected => selected.StartsWith(path));
                            },
                            currentRoot);
                        currentRoot.RecordFilters.Add(listing);
                    }
                    currentRoot = listing;
                }
            }
        }

        // Sort all record filters
        var queue = new Queue<RecordFilterListing>(root.RecordFilters);
        while (queue.Any()) {
            var listing = queue.Dequeue();
            queue.Enqueue(listing.RecordFilters);

            listing.RecordFilters.Sort();
        }

        return root.RecordFilters;
    }

    public static IEnumerable<RecordFilterListing> GetRecursiveListings<T>(this IEnumerable<T> elements,
        Func<T, string?> stringSelector,
        params char[] separator) {
        return elements.GetRecursiveListings(
            t => {
                var selector = stringSelector(t);
                return selector is not null ? selector.AsEnumerable() : Enumerable.Empty<string>();
            },
            separator);
    }
}
