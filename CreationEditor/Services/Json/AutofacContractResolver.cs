using Autofac;
using Autofac.Core;
using Newtonsoft.Json.Serialization;
namespace CreationEditor.Services.Json;

public sealed class AutofacContractResolver(IComponentContext componentContext) : DefaultContractResolver {
    protected override JsonObjectContract CreateObjectContract(Type objectType) {
        // use Autofac to create types that have been registered with it
        if (componentContext.IsRegistered(objectType)) {
            var contract = ResolveContact(objectType);
            contract.DefaultCreator = () => componentContext.Resolve(objectType);

            return contract;
        }

        return base.CreateObjectContract(objectType);
    }

    private JsonObjectContract ResolveContact(Type objectType) {
        // attempt to create the contact from the resolved type
        if (componentContext.ComponentRegistry.TryGetRegistration(new TypedService(objectType), out var registration)) {
            var viewType = registration.Activator.LimitType;
            if (viewType is not null) {
                return base.CreateObjectContract(viewType);
            }
        }

        // fall back to using the registered type
        return base.CreateObjectContract(objectType);
    }
}
