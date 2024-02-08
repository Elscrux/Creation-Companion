using Autofac;
using Autofac.Builder;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.Services.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Avalonia.ViewModels.Asset.Browser;
using CreationEditor.Avalonia.ViewModels.Mod;
using CreationEditor.Avalonia.ViewModels.Scripting;
using CreationEditor.Services.Archive;
using CreationEditor.Services.Asset;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Mod;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Plugin;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Records;
namespace CreationEditor.Avalonia.Modules;

public abstract class GameSpecificModule<TMod, TModGetter> : Module
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter> {

    protected abstract GameRelease GameRelease { get; }

    protected abstract IReg<IModInfoProvider<TModGetter>> ModInfoProvider { get; }
    protected abstract IReg<IRecordBrowserGroupProvider> RecordBrowserGroupProvider { get; }
    protected abstract IReg<IRecordProviderFactory> RecordProviderFactory { get; }
    protected abstract IReg<IRecordEditorFactory> RecordEditorFactory { get; }
    protected abstract IReg<IModGetterVM> ModGetterVM { get; }

    protected abstract IReg<ICellBrowserFactory> CellBrowserFactory { get; }
    protected abstract IReg<IApplicationIconProvider> ApplicationIconProvider { get; }

    protected abstract IReg<IArchiveService> ArchiveService { get; }
    protected abstract IReg<IAssetBrowserVM> AssetBrowserVM { get; }
    protected abstract IReg<IAssetTypeProvider> AssetTypeProvider { get; }

    protected abstract IReg<IScriptVM> ScriptVM { get; }

    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterType<AutofacPluginService<TMod, TModGetter>>()
            .As<IPluginService>()
            .SingleInstance();

        Register(builder, ScriptVM)
            .As<IScriptVM>();


        // General
        builder.RegisterType<EditorEnvironment<TMod, TModGetter>>()
            .As<ILinkCacheProvider>()
            .As<IEditorEnvironment>()
            .As<IEditorEnvironment<TMod, TModGetter>>()
            .SingleInstance();

        builder.RegisterType<EditorEnvironmentUpdater<TMod, TModGetter>>()
            .As<IEditorEnvironmentUpdater>();

        builder.RegisterInstance(new GameReleaseInjection(GameRelease))
            .AsImplementedInterfaces();

        // Controller
        builder.RegisterType<RecordController<TMod, TModGetter>>()
            .As<IRecordController>()
            .SingleInstance();

        // Provider
        Register(builder, ApplicationIconProvider)
            .As<IApplicationIconProvider>()
            .SingleInstance();

        Register(builder, ModInfoProvider)
            .As<IModInfoProvider<IModGetter>>()
            .As<IModInfoProvider<TModGetter>>();

        Register(builder, RecordBrowserGroupProvider)
            .As<IRecordBrowserGroupProvider>();

        builder.RegisterGeneric(typeof(RecordProvider<,>))
            .As(typeof(RecordProvider<,>));

        Register(builder, AssetTypeProvider)
            .As<IAssetTypeProvider>()
            .SingleInstance();

        // Factory
        Register(builder, RecordProviderFactory)
            .As<IRecordProviderFactory>();

        Register(builder, RecordEditorFactory)
            .As<IRecordEditorFactory>();

        Register(builder, CellBrowserFactory)
            .As<ICellBrowserFactory>();

        // Service
        Register(builder, ArchiveService)
            .As<IArchiveService>();

        // View Model
        Register(builder, ModGetterVM)
            .As<IModGetterVM>();

        Register(builder, AssetBrowserVM)
            .As<IAssetBrowserVM>();
    }

    private static IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>
        Register<T>(ContainerBuilder builder, IReg<T> x)
        where T : notnull {
        return builder.RegisterType(x.GetType().GenericTypeArguments[2]);
    }

    protected interface IReg<out T>;
    private sealed class Reg<T> : IReg<T>;
    protected static IReg<T> Register<T>() => new Reg<T>();
}
