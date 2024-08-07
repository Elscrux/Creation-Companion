using Autofac;
using CreationEditor.Avalonia.ViewModels.Query;
using CreationEditor.Services.Query;
using CreationEditor.Services.Query.From;
using CreationEditor.Services.Query.Select;
using CreationEditor.Services.Query.Where;
namespace CreationEditor.Avalonia.Modules;

public sealed class QueryModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<QueryRunner>()
            .As<IQueryRunner>();

        builder.RegisterType<QueryFromRecordType>()
            .AsSelf()
            .As<IQueryFrom>();

        builder.RegisterType<QueryCondition>()
            .As<IQueryCondition>();

        builder.RegisterType<ReflectionQueryFieldSelector>()
            .As<IQueryFieldSelector>();

        builder.RegisterType<QueryVM>()
            .AsSelf();

        builder.RegisterType<QueryFromFactory>()
            .As<IQueryFromFactory>()
            .SingleInstance();

        builder.RegisterType<QueryCompareFunctionFactory>()
            .As<IQueryCompareFunctionFactory>()
            .SingleInstance();

        builder.RegisterType<QueryConditionFactory>()
            .As<IQueryConditionFactory>()
            .SingleInstance();
    }
}
