using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class NpcExtension {
    extension(INpcGetter npc) {
        public MaleFemaleGender GetGender() {
            return (npc.Configuration.Flags & NpcConfiguration.Flag.Female) == 0 ? MaleFemaleGender.Male : MaleFemaleGender.Female;
        }
    }
}
