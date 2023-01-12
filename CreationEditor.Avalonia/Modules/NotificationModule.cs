using Autofac;
using CreationEditor.Avalonia.ViewModels.Notification;
using CreationEditor.Services.Notification;
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
