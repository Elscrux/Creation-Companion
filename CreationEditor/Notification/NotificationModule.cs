using Autofac;
namespace CreationEditor.Notification; 

public sealed class NotificationModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<Notifier>()
            .As<INotifier>()
            .SingleInstance();
    }
}
