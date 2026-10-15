using System.Runtime.CompilerServices;
using ReactiveUI.Builder;
namespace CreationEditor.Skyrim.Tests;

internal static class TestInitializer {
    [ModuleInitializer]
    internal static void Initialize() {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();
    }
}
