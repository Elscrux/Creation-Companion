using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim.Services.Record;

public class LightRecordRemappingPreprocessor(
    ILinkCacheProvider linkCacheProvider,
    IRecordController recordController) : IRecordRemappingPreprocessor {
    public string Description => "Keep light radius for placed lights";

    public bool IsApplicable(IReferencedRecord referencedRecord) {
        return referencedRecord.Record is ILightGetter;
    }

    public void PreprocessRemapping(IReferencedRecord record, FormKey remappingFormKey) {
        if (record.Record is not ILightGetter oldLight) return;
        if (!linkCacheProvider.LinkCache.TryResolve<ILightGetter>(remappingFormKey, out var newLight)) return;
        if (oldLight.Radius == newLight.Radius) return;

        var radiusFraction = (float) oldLight.Radius / newLight.Radius;
        foreach (var recordReference in record.RecordReferences) {
            if (linkCacheProvider.LinkCache.TryResolve<IPlacedObjectGetter>(recordReference.FormKey, out var lightRef)) {
                var placedObject = recordController.GetOrAddOverride<IPlacedObject, IPlacedObjectGetter>(lightRef);
                placedObject.Radius += radiusFraction;
            }
        }
    }
}
