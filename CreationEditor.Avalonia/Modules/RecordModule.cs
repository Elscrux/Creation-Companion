using Autofac;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record.Cache;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Services.Mutagen.References.Record.Query;
namespace CreationEditor.Avalonia.Modules;

public sealed class RecordModule : Module {
    protected override void Load(ContainerBuilder builder) {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Record References
        builder.RegisterType<RecordReferenceQuery>()
            .As<IRecordReferenceQuery>()
            .SingleInstance();

        builder.RegisterType<RecordReferenceCacheFactory>()
            .As<IRecordReferenceCacheFactory>()
            .SingleInstance();

        builder.RegisterType<RecordReferenceController>()
            .As<IRecordReferenceController>()
            .SingleInstance();

        // Record Browser
        builder.RegisterType<RecordFilterBuilder>()
            .As<IRecordFilterBuilder>();

        builder.RegisterType<RecordFilterProvider>()
            .As<IRecordFilterProvider>()
            .SingleInstance();

        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<IRecordFilter>()
            .As<IRecordFilter>()
            .AsSelf()
            .SingleInstance();

        builder.RegisterType<RecordTypeListing>()
            .AsSelf();

        builder.RegisterAssemblyTypes(assemblies)
            .AssignableTo<IRecordActionsProvider>()
            .AsSelf();

        builder.RegisterGeneric(typeof(RecordActionsProvider<,>))
            .AsSelf();

        builder.RegisterAssemblyTypes(assemblies)
            .Where(x => x.GetInterfaces().Contains(typeof(IRecordContextMenuProvider)))
            .AsSelf();

        builder.RegisterGeneric(typeof(RecordContextMenuProvider<,>))
            .AsSelf();

        // Record Editor
        builder.RegisterType<RecordEditorController>()
            .As<IRecordEditorController>()
            .SingleInstance();

        // Record Provider
        builder.RegisterType<RecordTypeProvider>()
            .AsSelf();

        builder.RegisterType<RecordIdentifiersProvider>()
            .AsSelf();
    }
}
