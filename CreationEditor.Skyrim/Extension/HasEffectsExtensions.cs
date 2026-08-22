using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim;

public static class HasEffectsExtensions {
    extension(IHasEffectsGetter spell) {
        public ActorValue? GetSchoolOfMagic(ILinkCache linkCache) {
            // Try to estimate the school of magic based on the most common spell effect school
            var magicSchool = spell.Effects
                .Select(x => x.BaseEffect.TryResolve(linkCache))
                .WhereNotNull()
                .MaxBy(x => x.MagicSkill);

            return magicSchool?.MagicSkill;
        }
        public uint GetMagicLevel(ILinkCache linkCache) {
            // OTry to estimate the magic level based on the highest minimum skill level of the spell's effects
            var max = spell.Effects
                .Select(e => e.BaseEffect.TryResolve(linkCache))
                .WhereNotNull()
                .Select(e => e.MinimumSkillLevel)
                .Max();

            return max;
        }
    }
}
