using Avalonia.Media;
using CreationEditor.Avalonia.Services.Avalonia;
using FluentAvalonia.UI.Windowing;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Views.Startup;

public sealed class StartupSplashScreen : IApplicationSplashScreen {
    public string AppName => "Creation Companion";
    [Reactive]public IImage AppIcon { get; set; }
    public object SplashScreenContent { get; }
    public int MinimumShowTime => 0;

    public StartupSplashScreen(IApplicationIconProvider applicationIconProvider) {
        AppIcon = applicationIconProvider.SpinningIcon;
        SplashScreenContent = new SplashScreen(this);
    }

    public Task RunTasks(CancellationToken cancellationToken) => Task.CompletedTask;
}
