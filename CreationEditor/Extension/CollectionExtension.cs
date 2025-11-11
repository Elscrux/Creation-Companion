namespace CreationEditor;

public static class CollectionExtension {
    extension<T>(ICollection<T> collection) {
        public void AddRange(IEnumerable<T> items) {
            foreach (var item in items) {
                collection.Add(item);
            }
        }
    }
}
