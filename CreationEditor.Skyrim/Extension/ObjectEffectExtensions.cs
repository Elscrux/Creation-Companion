using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim;

public static class ObjectEffectExtensions {
    public static ActorValue? GetSchoolOfMagic(this IObjectEffectGetter objectEffect, ILinkCache linkCache) {
        foreach (var effect in objectEffect.Effects) {
            var magicEffect = effect.BaseEffect.TryResolve(linkCache);
            if (magicEffect is null) continue;

            return magicEffect.MagicSkill;
        }

        return null;
    }

    public static uint GetMagicLevel(this IObjectEffectGetter objectEffect, ILinkCache linkCache) {
        var max = objectEffect.Effects
            .Select(e => e.BaseEffect.TryResolve(linkCache))
            .WhereNotNull()
            .Select(e => e.MinimumSkillLevel)
            .Max();

        return max;
    }
}

public static class SpellExtensions {
    public static ActorValue? GetSchoolOfMagic(this ISpellGetter spell, ILinkCache linkCache) {
        foreach (var effect in spell.Effects) {
            var magicEffect = effect.BaseEffect.TryResolve(linkCache);
            if (magicEffect is null) continue;

            return magicEffect.MagicSkill;
        }

        return null;
    }

    public static uint GetMagicLevel(this ISpellGetter spell, ILinkCache linkCache) {
        var max = spell.Effects
            .Select(e => e.BaseEffect.TryResolve(linkCache))
            .WhereNotNull()
            .Select(e => e.MinimumSkillLevel)
            .Max();

        return max;
    }
}
