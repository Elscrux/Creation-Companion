using Autofac;
using Autofac.Core;
using Newtonsoft.Json.Serialization;
namespace CreationEditor.Services.Json;

public sealed class AutofacContractResolver : DefaultContractResolver {
    private readonly IComponentContext _componentContext;

    public AutofacContractResolver(IComponentContext componentContext) {
        _componentContext = componentContext;
    }

    protected override JsonObjectContract CreateObjectContract(Type objectType) {
        // use Autofac to create types that have been registered with it
        if (_componentContext.IsRegistered(objectType)) {
            var contract = ResolveContact(objectType);
            contract.DefaultCreator = () => _componentContext.Resolve(objectType);

            return contract;
        }

        return base.CreateObjectContract(objectType);
    }

    private JsonObjectContract ResolveContact(Type objectType) {
        // attempt to create the contact from the resolved type
        if (_componentContext.ComponentRegistry.TryGetRegistration(new TypedService(objectType), out var registration)) {
            var viewType = registration.Activator.LimitType;
            if (viewType is not null) {
                return base.CreateObjectContract(viewType);
            }
        }

        // fall back to using the registered type
        return base.CreateObjectContract(objectType);
    }
}
