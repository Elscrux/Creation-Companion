using Avalonia.Media;
using CreationEditor.Services.Query;
using Mutagen.Bethesda.Plugins;
namespace MapperPlugin.Model;

public sealed record MarkingMappingMemento(
    bool Enable,
    bool UseQuery,
    bool UseRandomColorsInQuery,
    FormLinkInformation Record,
    QueryRunnerMemento QueryRunner,
    Color Color,
    float Size);
