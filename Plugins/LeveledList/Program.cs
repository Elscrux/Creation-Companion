using Autofac;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim.Avalonia.Modules;
using LeveledList;
using LeveledList.Services.LeveledList;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

var builder = new ContainerBuilder();

builder.RegisterModule<MutagenModule>();
builder.RegisterModule<EditorModule>();
builder.RegisterModule<SkyrimModule>();
builder.RegisterModule<LeveledListModule>();

var container = builder.Build();

var editorEnvironment = container.Resolve<IEditorEnvironment<ISkyrimMod, ISkyrimModGetter>>();

var modToLookAt = editorEnvironment.LinkCache.PriorityOrder[^1];
var mod = new SkyrimMod(ModKey.FromFileName("GeneratedLeveledLists.esp"), SkyrimRelease.SkyrimSE, 1.7f);

var featureProvider = container.Resolve<IFeatureProvider>();
var tierController = container.Resolve<ITierController>();

var generator = new LeveledListGenerator(featureProvider, tierController, editorEnvironment, modToLookAt, mod);
generator.Generate();

Console.WriteLine("Generation done!");
