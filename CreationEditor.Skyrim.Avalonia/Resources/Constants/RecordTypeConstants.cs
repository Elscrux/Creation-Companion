using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Resources.Constants;

public static class RecordTypeConstants {
    public static readonly IEnumerable<Type> RelatableTypes = typeof(IRelatableGetter).AsEnumerable();
    public static readonly IEnumerable<Type> OutfitTypes = typeof(IOutfitGetter).AsEnumerable();
    public static readonly IEnumerable<Type> LocationTypes = typeof(ILocationGetter).AsEnumerable();
    public static readonly IEnumerable<Type> FormListTypes = typeof(IFormListGetter).AsEnumerable();
    public static readonly IEnumerable<Type> WorldspaceTypes = typeof(IWorldspaceGetter).AsEnumerable();
    public static readonly IEnumerable<Type> WaterTypes = typeof(IWaterGetter).AsEnumerable();
    public static readonly IEnumerable<Type> CellTypes = typeof(ICellGetter).AsEnumerable();
    public static readonly IEnumerable<Type> RegionTypes = typeof(IRegionGetter).AsEnumerable();
    public static readonly IEnumerable<Type> QuestTypes = typeof(IQuestGetter).AsEnumerable();
    public static readonly IEnumerable<Type> GlobalVariableTypes = typeof(IGlobalGetter).AsEnumerable();
    public static readonly IEnumerable<Type> KeywordTypes = typeof(IKeywordGetter).AsEnumerable();
    public static readonly IEnumerable<Type> ObjectIdTypes = typeof(IObjectIdGetter).AsEnumerable();

    // Sound Descriptor
    public static readonly Type[] SoundDescriptorTypes = [typeof(ISoundDescriptorGetter)];

    // Static
    public static readonly Type[] StaticTypes = [typeof(IStaticGetter)];

    // Spell
    public static readonly Type[] SpellTypes = [typeof(ISpellGetter)];

    // Placed
    public static readonly Type PlacedBaseType = typeof(IPlacedGetter);
    public static readonly Type PlacedSimpleType = typeof(IPlacedSimpleGetter);
    public static readonly IEnumerable<Type> PlacedTypes = PlacedBaseType.AsEnumerable();
    public static readonly Type[] AllPlacedInterfaceTypes = [PlacedBaseType, PlacedSimpleType];

    // Placeable
    public static readonly Type[] PlaceableObjectTypes = [
        typeof(IAcousticSpaceGetter),
        typeof(IActivatorGetter),
        typeof(IAddonNodeGetter),
        typeof(IAlchemicalApparatusGetter),
        typeof(IAmmunitionGetter),
        typeof(IArmorGetter),
        typeof(IArtObjectGetter),
        typeof(IBookGetter),
        typeof(IContainerGetter),
        typeof(IDoorGetter),
        typeof(IFloraGetter),
        typeof(IFurnitureGetter),
        typeof(IIdleMarkerGetter),
        typeof(IIngestibleGetter),
        typeof(IIngredientGetter),
        typeof(IKeyGetter),
        typeof(ILightGetter),
        typeof(IMiscItemGetter),
        typeof(IMoveableStaticGetter),
        typeof(IScrollGetter),
        typeof(ISoulGemGetter),
        typeof(ISoundMarkerGetter),
        typeof(ISpellGetter),
        typeof(IStaticGetter),
        typeof(ITalkingActivatorGetter),
        typeof(ITextureSetGetter),
        typeof(ITreeGetter),
        typeof(IWeaponGetter),
    ];

    public static readonly Type[] PlaceableTypes = [
        typeof(INpcGetter),
        typeof(IProjectileGetter),
        typeof(IHazardGetter),
        ..PlaceableObjectTypes,
    ];

    public static readonly Type[] PlacedNpcTypes = [typeof(IPlacedNpcGetter)];

    // Package
    public static readonly IReadOnlyList<Type> PackageDataNumericTypes = [typeof(bool), typeof(float), typeof(int)];
    public static readonly IReadOnlyList<Type> PackageDataLocationTypes = LocationTypes.Concat(AllPlacedInterfaceTypes).ToArray();
    public static readonly IReadOnlyList<Type> PackageDataTypes = PackageDataNumericTypes.Concat(PackageDataLocationTypes).ToArray();

    // Alias
    public static readonly IReadOnlyList<Type> AllAliasTypes
        = AllPlacedInterfaceTypes
            .Concat(LocationTypes)
            .ToArray();
}
