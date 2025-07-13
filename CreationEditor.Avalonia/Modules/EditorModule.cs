using Autofac;
namespace CreationEditor.Avalonia.Modules;

public sealed class EditorModule : Module {
    protected override void Load(ContainerBuilder builder) {
        base.Load(builder);

        builder.RegisterModule<MainModule>();
        builder.RegisterModule<NotificationModule>();
        builder.RegisterModule<QueryModule>();
        builder.RegisterModule<AssetModule>();
        builder.RegisterModule<RecordModule>();
        builder.RegisterModule<ReferenceModule>();
        builder.RegisterModule<NewtonsoftJsonModule>();
        builder.RegisterModule<LoggingModule>();
        builder.RegisterModule<SettingsModule>();
    }
}
