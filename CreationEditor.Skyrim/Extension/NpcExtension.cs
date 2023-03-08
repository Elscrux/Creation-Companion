using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
namespace CreationEditor.Skyrim;

public static class NpcExtension {
    public static MaleFemaleGender GetGender(this INpcGetter npc) {
        return (npc.Configuration.Flags & NpcConfiguration.Flag.Female) == 0 ? MaleFemaleGender.Male : MaleFemaleGender.Female;
    }
}
