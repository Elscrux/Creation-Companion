using Autofac;
using Avalonia;
using Avalonia.Markup.Xaml;
using CreationEditor.Avalonia.Services.Exceptions;
namespace CreationEditor.Skyrim.Avalonia;

public class App : Application {
    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
        Program.Host.Container.Resolve<UiCrashHandler>().Attach();

        base.OnFrameworkInitializationCompleted();
    }
}
