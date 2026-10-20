using CreationEditor.Services.Mutagen.Type;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda.Environments.DI;
using ReactiveUI;
namespace CreationEditor.Avalonia.Services.Actions.Global;

/// <summary>
/// Provides one "Create X Record" action per registered record type.
/// Placed record types are excluded, as there are too many of them.
/// </summary>
public sealed class RecordCreationGlobalActionsProvider(
    IMutagenTypeProvider mutagenTypeProvider,
    IGameReleaseContext gameReleaseContext,
    IMutagenCommonAspectsProvider mutagenCommonAspectsProvider,
    IGenericContextActionsProvider genericContextActionsProvider) : IGlobalActionsProvider {

    public IEnumerable<GlobalAction> GetActions() {
        return mutagenTypeProvider
            .GetRegistrations(gameReleaseContext.Release)
            .Where(x => !mutagenCommonAspectsProvider.PlacedType.IsAssignableFrom(x.GetterType))
            .Select(registration => new GlobalAction(
                $"Record.Create.{registration.GetterType.Name}",
                $"Create {mutagenTypeProvider.GetTypeName(registration.GetterType)} Record",
                ReactiveCommand.Create(() => {
                    genericContextActionsProvider.CreateNewRecord(registration.GetterType);
                }),
                "Record",
                FASymbol.Add,
                -100));
    }
}
