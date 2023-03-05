using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Extension;
using Noggog;
namespace CreationEditor.Avalonia.Services.Record.Browser.Filter;

public static class RecordFilterExtension {
    public static IEnumerable<RecordFilterListing> GetRecursiveListings<T>(this IEnumerable<T> elements,
        char separator,
        Func<T, string?> stringSelector) {

        var root = new RecordFilterListing(string.Empty, _ => true);

        foreach (var s in elements) {
            var accumulatedPath = string.Empty;
            var currentRoot = root;

            var selectedString = stringSelector(s);
            if (selectedString == null) continue;

            var directories = selectedString.Split(separator);
            for (var i = 0; i < directories.Length; i++) {
                var directory = directories[i];
                if (directory.IsNullOrEmpty()) break;

                accumulatedPath += i == directories.Length - 1 ? directory : directory + separator;

                var listing = currentRoot.RecordFilters.FirstOrDefault(x => string.Equals(x.DisplayName, directory, StringComparison.OrdinalIgnoreCase));
                if (listing == null) {
                    var path = accumulatedPath;
                    listing = new RecordFilterListing(
                        directory,
                        record => {
                            if (record is not T t) return false;

                            var selector = stringSelector(t);
                            return selector != null && selector.StartsWith(path);

                        },
                        currentRoot);
                    currentRoot.RecordFilters.Add(listing);
                }
                currentRoot = listing;
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
}
