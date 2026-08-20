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
            // Try to get school of magic via the HalfCostPerk, if it exists
            var halfCostPerk = spell.HalfCostPerk.TryResolve(linkCache);
            if (halfCostPerk?.EditorID is {} perkEditorId) {
                // Find first word of the perk editor ID, like AlterationApprentice25, and try to parse it as an ActorValue
                var span = perkEditorId.AsSpan();
                var endIndex = span.Length;
                for (var i = 1; i < span.Length; i++) {
                    if (!char.IsUpper(span[i])) continue;

                    endIndex = i;
                    break;
                }

                var firstNamePart = span[..endIndex].ToString();
                if (Enum.TryParse<ActorValue>(firstNamePart, out var actorValue)) {
                    return actorValue;
                }
            }

            // Otherwise, try to estimate the school of magic based on the most common spell effect school
            var magicSchool = spell.Effects
                .Select(x => x.BaseEffect.TryResolve(linkCache))
                .WhereNotNull()
                .MaxBy(x => x.MagicSkill);

            return magicSchool?.MagicSkill;
        }
        public uint GetMagicLevel(ILinkCache linkCache) {
            // Try to get magic level via the HalfCostPerk, if it exists
            var halfCostPerk = spell.HalfCostPerk.TryResolve(linkCache);
            if (halfCostPerk?.EditorID is {} perkEditorId) {
                // Get number at the end of the perk editor ID, like AlterationApprentice25
                var span = perkEditorId.AsSpan();
                var endIndex = span.Length;
                while (endIndex > 0 && char.IsNumber(span[endIndex - 1])) {
                    endIndex--;
                }
                
                var digitSpan = span[endIndex..];
                
                if (uint.TryParse(digitSpan, out var magicLevel)) {
                    return magicLevel;
                }
            }

            // Otherwise, try to estimate the magic level based on the highest minimum skill level of the spell's effects
            var max = spell.Effects
                .Select(e => e.BaseEffect.TryResolve(linkCache))
                .WhereNotNull()
                .Select(e => e.MinimumSkillLevel)
                .Max();

            return max;
        }
    }
}
