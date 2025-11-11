namespace CreationEditor;

public static class TreeExtension {
    extension<T>(T root) {
        public IEnumerable<T> GetTreeLeaves(Func<T, IEnumerable<T>?> childSelector) {
            if (childSelector(root)?.Any() is false) {
                yield return root;
                yield break;
            }

            var stack = new Stack<T>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();

                var childEnumerable = childSelector(current);
                if (childEnumerable is null || !childEnumerable.Any()) {
                    yield return current;
                    continue;
                }

                foreach (var child in childEnumerable) {
                    stack.Push(child);
                }
            }
        }
        public IEnumerable<TLimit> GetAllChildren<TLimit>(
            Func<T, IEnumerable<T>?> childSelector,
            bool includeRoot = false)
            where TLimit : T {
            return root.GetAllChildren(childSelector, includeRoot).OfType<TLimit>();
        }
        public IEnumerable<T> GetAllChildren(Func<T, IEnumerable<T>?> childSelector, bool includeRoot = false) {
            if (includeRoot) yield return root;

            var stack = new Stack<T>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();

                var childEnumerable = childSelector(current);
                if (childEnumerable is null) continue;

                foreach (var child in childEnumerable) {
                    stack.Push(child);
                    yield return child;
                }
            }
        }
        public IEnumerable<TLimit> GetChildren<TLimit>(
            Predicate<T> childPredicate,
            Func<T, IEnumerable<T>?> childSelector,
            bool includeRoot = false)
            where TLimit : T {
            return root.GetChildren(childPredicate, childSelector, includeRoot).OfType<TLimit>();
        }
        public IEnumerable<T> GetChildren(
            Predicate<T> childPredicate,
            Func<T, IEnumerable<T>?> childSelector,
            bool includeRoot = false) {
            if (includeRoot) yield return root;

            var stack = new Stack<T>();
            stack.Push(root);

            while (stack.Count > 0) {
                var current = stack.Pop();

                var childEnumerable = childSelector(current);
                if (childEnumerable is null) continue;

                foreach (var child in childEnumerable) {
                    if (childPredicate(child)) {
                        stack.Push(child);
                        yield return child;
                    }
                }
            }
        }
    }

    extension<T>(IEnumerable<T> rootEnumerable) {
        public IEnumerable<T> GetAllChildren(
            Func<T, IEnumerable<T>?> childSelector,
            bool includeRoot = false) {
            return rootEnumerable.SelectMany(rootItem => GetAllChildren(rootItem, childSelector, includeRoot));
        }
        public IEnumerable<T> GetChildren(
            Predicate<T> childPredicate,
            Func<T, IEnumerable<T>?> childSelector,
            bool includeRoot = false) {
            return rootEnumerable.SelectMany(rootItem => GetChildren(rootItem, childPredicate, childSelector, includeRoot));
        }
    }
}
