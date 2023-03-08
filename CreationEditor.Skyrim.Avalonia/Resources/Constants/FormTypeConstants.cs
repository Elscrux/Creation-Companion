using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Resources.Constants;

public static class FormTypeConstants {
    public static readonly IEnumerable<Type> RelatableTypes = new[] { typeof(IFactionGetter), typeof(IRaceGetter) };
    public static readonly IEnumerable<Type> OutfitTypes = typeof(IOutfitGetter).AsEnumerable();
    public static readonly IEnumerable<Type> FormListTypes = typeof(IFormListGetter).AsEnumerable();
    public static readonly IEnumerable<Type> WorldspaceTypes = typeof(IWorldspaceGetter).AsEnumerable();
}
