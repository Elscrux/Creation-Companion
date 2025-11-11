using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim;

public static class ObjectEffectExtensions {
    extension(IObjectEffectGetter objectEffect) {
        public ActorValue? GetSchoolOfMagic(ILinkCache linkCache) {
            foreach (var effect in objectEffect.Effects) {
                var magicEffect = effect.BaseEffect.TryResolve(linkCache);
                if (magicEffect is null) continue;

                return magicEffect.MagicSkill;
            }

            return null;
        }
        public uint GetMagicLevel(ILinkCache linkCache) {
            var max = objectEffect.Effects
                .Select(e => e.BaseEffect.TryResolve(linkCache))
                .WhereNotNull()
                .Select(e => e.MinimumSkillLevel)
                .Max();

            return max;
        }
    }

}

public static class SpellExtensions {
    extension(ISpellGetter spell) {
        public ActorValue? GetSchoolOfMagic(ILinkCache linkCache) {
            foreach (var effect in spell.Effects) {
                var magicEffect = effect.BaseEffect.TryResolve(linkCache);
                if (magicEffect is null) continue;

                return magicEffect.MagicSkill;
            }

            return null;
        }
        public uint GetMagicLevel(ILinkCache linkCache) {
            var max = spell.Effects
                .Select(e => e.BaseEffect.TryResolve(linkCache))
                .WhereNotNull()
                .Select(e => e.MinimumSkillLevel)
                .Max();

            return max;
        }
    }

}
