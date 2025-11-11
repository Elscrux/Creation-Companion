namespace CreationEditor.Services.Query.Where;

public static class QueryConditionExtensions {
    extension(IEnumerable<IQueryCondition> conditions) {
        public bool EvaluateConditions(object? obj) {
            if (obj is null) return false;

            var andStack = new Stack<bool>();

            foreach (var condition in conditions) {
                // Evaluate condition result
                var result = condition.Evaluate(obj);
                if (condition.Negate) result = !result;

                // Push current condition result to stack
                andStack.Push(result);

                // If condition is an OR, check if all conditions in the stack are true
                if (condition.IsOr && ValidateAndStack()) return true;
            }

            return andStack.Count > 0 && ValidateAndStack();

            bool ValidateAndStack() {
                // Check if all conditions in the stack are true
                // If so, we have a valid and block in our or block, so return true
                if (andStack.All(x => x)) return true;

                // Otherwise, clear the AND stack and continue
                andStack.Clear();
                return false;
            }
        }
    }
}
