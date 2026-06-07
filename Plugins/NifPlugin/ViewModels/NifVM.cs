using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using CreationEditor;
using CreationEditor.Avalonia.Services.Avalonia;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Services.DataSource;
using DynamicData.Binding;
using NiflySharp;
using NifPlugin.Models;
using NifPlugin.Services;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace NifPlugin.ViewModels;

public sealed partial class NifVM : ViewModel {
    private readonly ILogger _logger;
    public ITaskDialogProvider TaskDialogProvider { get; }
    public NifEditVertexColorService NifEditVertexColorService { get; }
    public DataSourceFileLink NifFileLink { get; }
    public NifFile NifFile { get; }
    public IObservableCollection<NifBlock> Blocks { get; }
    public HierarchicalTreeDataGridSource<NifBlock> BlocksTreeSource { get; }

    public NifVM(
        ILogger logger,
        ITaskDialogProvider taskDialogProvider,
        NifEditVertexColorService nifEditVertexColorService,
        DataSourceFileLink nifFileLink) {
        _logger = logger;
        TaskDialogProvider = taskDialogProvider;
        NifEditVertexColorService = nifEditVertexColorService;
        NifFileLink = nifFileLink;

        using var readFileStream = NifFileLink.ReadFileStream();
        if (readFileStream is null) {
            throw new Exception("Could not open NIF file");
        }

        var nif = new NifFile(readFileStream);
        nif.Load(nifFileLink.FullPath);
        NifFile = nif;

        Blocks = new ObservableCollectionExtended<NifBlock>(GetBlocks(nif));

        BlocksTreeSource = new HierarchicalTreeDataGridSource<NifBlock>(Blocks) {
            Columns = {
                new HierarchicalExpanderColumn<NifBlock>(
                    new TemplateColumn<NifBlock>(
                        "Id",
                        new FuncDataTemplate<NifBlock>((block, _) => {
                            if (block is null) return null;

                            return new TextBlock { Text = block.Id + ": " + block.NiObject.GetType().Name };
                        })),
                    b => b.Children
                ),
                new TemplateColumn<NifBlock>(
                    "Name",
                    new FuncDataTemplate<NifBlock>((block, _) => {
                        if (block is null) return null;

                        var stringRef = block.NiObject.StringRefs.FirstOrDefault();
                        if (stringRef is null) return null;

                        return new TextBlock { Text = stringRef.String };
                    })),
            }
        };
    }

    public static IReadOnlyList<NifBlock> GetBlocks(NifFile nif) {
        var blocks = new Dictionary<int, NifBlock>();

        foreach (var nifBlock in nif.Blocks) {
            if (!nif.GetBlockIndex(nifBlock, out var blockId)) continue;

            if (!blocks.TryGetValue(blockId, out var block)) {
                block = new NifBlock(blockId, nifBlock);
            }

            foreach (var reference in block.NiObject.References) {
                if (reference.Index == int.MinValue) continue;

                if (!blocks.TryGetValue(reference.Index, out var childBlock)) {
                    var niObject = nif.GetBlock(reference.Index);
                    if (niObject is null) continue;

                    childBlock = new NifBlock(reference.Index, niObject);
                }

                block.Children.Add(childBlock);
                blocks.TryAdd(reference.Index, childBlock);
            }

            blocks.TryAdd(blockId, block);
        }

        var blockList = new List<NifBlock>();
        var topLevelBlocks = blocks.Keys.ToList();
        foreach (var (_, block) in blocks) {
            foreach (var childBlock in block.Children) {
                topLevelBlocks.Remove(childBlock.Id);
            }
        }

        foreach (var topLevelBlock in topLevelBlocks) {
            if (blocks.TryGetValue(topLevelBlock, out var block)) {
                blockList.Add(block);
            }
        }

        return blockList;
    }

    [ReactiveCommand]
    public void SaveNif() {
        try {
            NifFile.Save(NifFileLink.FullPath);
        } catch (Exception e) {
            _logger.Here().Error(e, "Couldn't save modified nif {File}: {Exception}", NifFileLink.FullPath, e);
        }
    }

    [ReactiveCommand]
    public void ShiftHue(INiObject block, double hue) {
        NifEditVertexColorService.VertexColorHueShift(block, hue);
    }

    [ReactiveCommand]
    public void ShiftSaturation(INiObject block, double saturation) {
        NifEditVertexColorService.VertexColorSaturationBoost(block, saturation);
    }

    [ReactiveCommand]
    public void ShiftLightness(INiObject block, double lightness) {
        NifEditVertexColorService.VertexColorLightness(block, lightness);
    }
}
