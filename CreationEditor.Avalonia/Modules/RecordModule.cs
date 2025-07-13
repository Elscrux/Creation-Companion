using Autofac;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Record.Browser.Filter;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.Services.Record.Provider;
using CreationEditor.Services.Mutagen.Decorations;
using CreationEditor.Services.Mutagen.Record;
namespace CreationEditor.Avalonia.Modules;

public sealed class RecordModule : Module {
    protected override void Load(ContainerBuilder builder) {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Record Decoration
        builder.RegisterType<RecordDecorationController>()
            .As<IRecordDecorationController>()
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
            .AssignableTo<IContextActionsProvider>()
            .As<IContextActionsProvider>()
            .SingleInstance();

        builder.RegisterType<InjectedContextMenuProvider>()
            .As<IContextMenuProvider>();

        // Record Editor
        builder.RegisterType<RecordEditorController>()
            .As<IRecordEditorController>()
            .SingleInstance();

        builder.RegisterGeneric(typeof(RecordEditorCore<,,>))
            .As(typeof(IRecordEditorCore<,,>))
            .AsSelf();

        builder.RegisterGeneric(typeof(RecordEditorHistory<,,>))
            .AsSelf();

        builder.RegisterGeneric(typeof(RecordEditorValidator<,,>))
            .AsSelf();

        // Record Provider
        builder.RegisterType<RecordTypeProvider>()
            .AsSelf();

        builder.RegisterType<RecordIdentifiersProvider>()
            .AsSelf();

        // Record Validator
        builder.RegisterType<DebugRecordValidator>()
            .As<IRecordValidator>()
            .AsSelf()
            .SingleInstance();
    }
}
