using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Resources.Constants;

public static class RecordTypeConstants {
    public static readonly IEnumerable<Type> RelatableTypes = typeof(IRelatableGetter).AsEnumerable();
    public static readonly IEnumerable<Type> OutfitTypes = typeof(IOutfitGetter).AsEnumerable();
    public static readonly IEnumerable<Type> LocationTypes = typeof(ILocationGetter).AsEnumerable();
    public static readonly IEnumerable<Type> FormListTypes = typeof(IFormListGetter).AsEnumerable();
    public static readonly IEnumerable<Type> WorldspaceTypes = typeof(IWorldspaceGetter).AsEnumerable();
    public static readonly IEnumerable<Type> CellTypes = typeof(ICellGetter).AsEnumerable();
    public static readonly IEnumerable<Type> GlobalVariableTypes = typeof(IGlobalGetter).AsEnumerable();
    public static readonly IEnumerable<Type> KeywordTypes = typeof(IKeywordGetter).AsEnumerable();
    public static readonly IEnumerable<Type> ObjectIdTypes = typeof(IObjectIdGetter).AsEnumerable();

    // Placed
    public static readonly Type PlacedBaseType = typeof(IPlacedGetter);
    public static readonly Type PlacedSimpleType = typeof(IPlacedSimpleGetter);
    public static readonly IEnumerable<Type> PlacedTypes = PlacedBaseType.AsEnumerable();
    public static readonly Type[] AllPlacedInterfaceTypes = { PlacedBaseType, PlacedSimpleType };

    // Package
    public static readonly Type[] PackageDataNumericTypes = { typeof(bool), typeof(float), typeof(int) };
    public static readonly Type[] PackageDataLocationTypes = LocationTypes.Concat(AllPlacedInterfaceTypes).ToArray();
    public static readonly Type[] PackageDataTypes = PackageDataNumericTypes.Concat(PackageDataLocationTypes).ToArray();

    // Alias
    public static readonly Type[] AllAliasTypes
        = AllPlacedInterfaceTypes
            .Concat(LocationTypes)
            .ToArray();
}
