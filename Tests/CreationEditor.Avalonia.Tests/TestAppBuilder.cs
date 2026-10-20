using System.Runtime.CompilerServices;
using Avalonia.Headless.XUnit;
using ReactiveUI;
using ReactiveUI.Avalonia;
using ReactiveUI.Builder;
using Splat;
[assembly: AvaloniaTestFramework]
namespace CreationEditor.Avalonia.Tests;

internal static class Initializer {
    [ModuleInitializer]
    public static void Initialize() {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithPlatformServices()
            .WithCoreServices()
            .BuildApp();

        // Register the Avalonia activation for view fetcher, normally registered by ReactiveUI.Avalonia
        Locator.CurrentMutable.Register(() => new AvaloniaActivationForViewFetcher(), typeof(IActivationForViewFetcher));
    }
}
