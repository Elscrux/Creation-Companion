// See https://aka.ms/new-console-template for more information

using System.IO.Abstractions;
using Autofac;
using CreationEditor.Avalonia.Modules;
using CreationEditor.Services.Asset;
using CreationEditor.Services.DataSource;
using CreationEditor.Services.Environment;
using CreationEditor.Skyrim.Avalonia.Modules;
using Mutagen.Bethesda.Autofac;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
var builder = new ContainerBuilder();

builder.RegisterModule<MutagenModule>();
builder.RegisterModule<EditorModule>();
builder.RegisterModule<SkyrimModule>();

var container = builder.Build();

var editorEnvironment = container.Resolve<IEditorEnvironment>();
editorEnvironment.Update(updater => updater
    .ActiveMod.New("TestMod")
    .LoadOrder.AddImmutableMods(
        Skyrim.ModKey,
        Update.ModKey,
        Dawnguard.ModKey,
        HearthFires.ModKey,
        Dragonborn.ModKey,
        ModKey.FromFileName("BSAssets.esm"),
        ModKey.FromFileName("BSAtmora.esm"),
        ModKey.FromFileName("BSArgonia.esm"),
        ModKey.FromFileName("BSHeartland.esm"),
        ModKey.FromFileName("BSIliacBay.esm"),
        ModKey.FromFileName("BSMorrowind.esm"),
        ModKey.FromFileName("BSRoscrea.esm"))
    .Build());

var assetController = container.Resolve<IAssetController>();
var dataDirectoryProvider = container.Resolve<IDataDirectoryProvider>();
var dataDirectorySource = new FileSystemDataSource(new FileSystem(), dataDirectoryProvider.Path);
assetController.Move(
    new FileSystemLink(dataDirectorySource, @"Textures\Clothes\Gag\gag2.dds"),
    new FileSystemLink(dataDirectorySource, @"Textures\Clothes\Gag\gag.dds"));

Console.WriteLine();
