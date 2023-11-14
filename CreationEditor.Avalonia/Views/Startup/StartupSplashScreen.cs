using Avalonia.Media;
using FluentAvalonia.UI.Windowing;
namespace CreationEditor.Avalonia.Views.Startup;

public sealed class StartupSplashScreen : IApplicationSplashScreen {
    public string AppName => "Creation Companion";
    public IImage AppIcon => null!;
    public object SplashScreenContent { get; } = new SplashScreen();
    public int MinimumShowTime => 0;

    public Task RunTasks(CancellationToken cancellationToken) => Task.CompletedTask;
}
