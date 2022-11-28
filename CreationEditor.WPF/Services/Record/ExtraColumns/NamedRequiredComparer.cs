using CreationEditor.WPF.Models.Record;
using Mutagen.Bethesda.Plugins.Aspects;
namespace CreationEditor.WPF.Services.Record;

public class NamedRequiredComparer : ReferencedComparer {
    public static readonly NamedRequiredComparer Instance = new();

    public override int Compare(IReferencedRecord? x, IReferencedRecord? y) {
        if (x?.Record is INamedRequiredGetter m1 && y?.Record is INamedRequiredGetter m2) {
            return StringComparer.OrdinalIgnoreCase.Compare(m1.Name, m2.Name);
        }

        throw new ArgumentException($"Can't compare {x} and {y}, one of them is not INamedGetter");
    }
}