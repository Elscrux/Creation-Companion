using Autofac;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Notification;
namespace CreationEditor.Avalonia.Modules; 

public sealed class NotificationModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<NotificationService>()
            .As<INotificationService>()
            .SingleInstance();
        
        builder.RegisterType<NotificationVM>()
            .As<INotificationVM>();
    }
}
