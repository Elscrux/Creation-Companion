using Autofac;
using CreationEditor.Services.Json;
using Newtonsoft.Json.Serialization;
namespace CreationEditor.Avalonia.Modules; 

public sealed class NewtonsoftJsonModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<AutofacContractResolver>()
            .As<IContractResolver>();
    }
}
