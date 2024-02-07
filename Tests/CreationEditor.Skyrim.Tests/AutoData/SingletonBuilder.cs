using System.Reflection;
using AutoFixture.Kernel;
namespace CreationEditor.Skyrim.Tests.AutoData;

public sealed class SingletonBuilder<TInterface, TActual>(ISpecimenBuilder builder) : ISpecimenBuilder {
    private readonly Lazy<TActual> _value = new(() => (TActual) new SpecimenContext(builder).Resolve(typeof(TActual)));

    public object Create(object request, ISpecimenContext context) {
        switch (request) {
            case SeededRequest seededRequest:
                if (CheckType(seededRequest.Request)) return Value();

                break;
            case ParameterInfo parameterInfo:
                if (CheckType(parameterInfo.ParameterType)) return Value();

                break;
        }

        return new NoSpecimen();

        bool CheckType(object obj) => obj is Type type && (type == typeof(TInterface) || type == typeof(TActual));
        object Value() => _value.Value ?? throw new InvalidOperationException($"Failed to create {typeof(TActual).Name}");
    }
}
