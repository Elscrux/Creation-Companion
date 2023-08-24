using Autofac;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Record.Actions;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels.Record.Provider;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Services.Mutagen.References.Record.Controller;
using CreationEditor.Services.Mutagen.References.Record.Query;
namespace CreationEditor.Avalonia.Modules;

public sealed class RecordModule : Module {
    protected override void Load(ContainerBuilder builder) {
        builder.RegisterType<RecordReferenceQuery>()
            .As<IRecordReferenceQuery>()
            .SingleInstance();

        builder.RegisterType<RecordFilterBuilder>()
            .As<IRecordFilterBuilder>();

        builder.RegisterType<RecordTypeListing>()
            .AsSelf();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
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

        // Controller
        builder.RegisterType<RecordReferenceController>()
            .As<IRecordReferenceController>()
            .SingleInstance();

        builder.RegisterType<RecordEditorController>()
            .As<IRecordEditorController>()
            .SingleInstance();

        // Provider
        builder.RegisterType<RecordTypeProvider>()
            .AsSelf();

        builder.RegisterType<RecordIdentifiersProvider>()
            .AsSelf();

        builder.RegisterType<RecordFilterProvider>()
            .As<IRecordFilterProvider>()
            .SingleInstance();
    }
}
