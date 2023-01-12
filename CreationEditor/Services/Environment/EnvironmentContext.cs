using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs;
namespace CreationEditor.Services.Environment;

public class EnvironmentContext : IEnvironmentContext {
    public IGameReleaseContext GameReleaseContext { get; }
    public IDataDirectoryProvider DataDirectoryProvider  { get; }
    
    private EnvironmentContext(
        IGameReleaseContext gameReleaseContext,
        IDataDirectoryProvider dataDirectoryProvider) {
        GameReleaseContext = gameReleaseContext;
        DataDirectoryProvider = dataDirectoryProvider;
    }

    public static EnvironmentContext Build(GameRelease gameRelease) {
        return new EnvironmentContext(
            new GameReleaseInjection(gameRelease),
            new DataDirectoryInjection(GameLocations.GetDataFolder(gameRelease)));
    }
}
