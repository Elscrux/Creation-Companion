using Mutagen.Bethesda.Environments.DI;
namespace CreationEditor.Services.Environment;

public interface IEnvironmentContext {
    public IGameReleaseContext GameReleaseContext { get; }
    public IDataDirectoryProvider DataDirectoryProvider { get; }
}
