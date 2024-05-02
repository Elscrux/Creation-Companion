using Mutagen.Bethesda.Plugins;
namespace MapperPlugin.Model;

public sealed record MapperMemento(
    string? ImagePath,
    FormKey Worldspace,
    int LeftCell,
    int RightCell,
    int TopCell,
    int BottomCell,
    int MarkingSize,
    List<MarkingMappingMemento> MarkingMappings);
