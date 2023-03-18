using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using CreationEditor.Avalonia;
using CreationEditor.Avalonia.Views;
using CreationEditor.Skyrim.Avalonia.Resources.Converter;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Picker;

public partial class PlacedPicker : LoadedUserControl {
    public static readonly StyledProperty<ILinkCache?> LinkCacheProperty
        = AvaloniaProperty.Register<PlacedPicker, ILinkCache?>(nameof(LinkCache));

    public static readonly StyledProperty<IFormLinkGetter?> PlacedProperty
        = AvaloniaProperty.Register<PlacedPicker, IFormLinkGetter?>(nameof(Placed));

    public static readonly StyledProperty<FormKey> CellProperty
        = AvaloniaProperty.Register<PlacedPicker, FormKey>(nameof(Cell));

    public static readonly StyledProperty<IList<IMajorRecordGetter>> PlacedRecordsProperty
        = AvaloniaProperty.Register<PlacedPicker, IList<IMajorRecordGetter>>(nameof(PlacedRecords));

    public static readonly StyledProperty<Func<IMajorRecordIdentifier, ILinkCache?, string?>> PlacedNameSelectorProperty
        = AvaloniaProperty.Register<PlacedPicker, Func<IMajorRecordIdentifier, ILinkCache?, string?>>(nameof(PlacedNameSelector));

    public static readonly StyledProperty<Func<IMajorRecordIdentifier, ILinkCache?, string?>> CellNameSelectorProperty
        = AvaloniaProperty.Register<PlacedPicker, Func<IMajorRecordIdentifier, ILinkCache?, string?>>(nameof(CellNameSelector));

    public static readonly StyledProperty<Func<IPlacedGetter, bool>> FilterProperty
        = AvaloniaProperty.Register<PlacedPicker, Func<IPlacedGetter, bool>>(nameof(Filter), _ => true);

    public ILinkCache? LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public IFormLinkGetter? Placed {
        get => GetValue(PlacedProperty);
        set => SetValue(PlacedProperty, value);
    }

    public FormKey Cell {
        get => GetValue(CellProperty);
        set => SetValue(CellProperty, value);
    }
    public IList<IMajorRecordGetter> PlacedRecords {
        get => GetValue(PlacedRecordsProperty);
        set => SetValue(PlacedRecordsProperty, value);
    }

    public Func<IMajorRecordIdentifier, ILinkCache?, string?> PlacedNameSelector {
        get => GetValue(PlacedNameSelectorProperty);
        set => SetValue(PlacedNameSelectorProperty, value);
    }

    public Func<IMajorRecordIdentifier, ILinkCache?, string?> CellNameSelector {
        get => GetValue(CellNameSelectorProperty);
        set => SetValue(CellNameSelectorProperty, value);
    }

    public Func<IPlacedGetter, bool> Filter {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public PlacedPicker() {
        InitializeComponent();

        PlacedNameSelector = (identifier, linkCache) => identifier is IPlacedGetter placed
            ? PlacedConverters.ToName.Convert(placed, linkCache) as string
            : linkCache != null && linkCache.TryResolve<IPlacedGetter>(identifier.FormKey, out var referencedPlaced)
                ? PlacedConverters.ToName.Convert(referencedPlaced, linkCache) as string
                : null;

        CellNameSelector = (identifier, linkCache) => linkCache != null && linkCache.TryResolve<ICellGetter>(identifier.FormKey, out var cell)
            ? CellConverters.ToName.Convert(cell, linkCache) as string
            : null;
    }

    protected override void OnLoaded() {
        base.OnLoaded();

        this.WhenAnyValue(x => x.Placed)
            .CombineLatest(this.WhenAnyValue(x => x.LinkCache), (placed, linkCache) => (Placed: placed, LinkCache: linkCache))
            .Subscribe(x => {
                if (x.LinkCache == null || x.Placed == null) return;

                if (x.LinkCache.TryResolveSimpleContext<IPlacedGetter>(x.Placed.FormKey, out var placed)) {
                    if (placed.Parent?.Record is ICellGetter cell) {
                        Cell = cell.FormKey;
                    }
                }
            });

        PlacedRecords = this.WhenAnyValue(x => x.Cell)
            .DistinctUntilChanged()
            .Select(cell => LinkCache != null && LinkCache.TryResolve<ICellGetter>(cell, out var record)
                ? record.Temporary
                    .Concat(record.Persistent)
                    .Where(Filter)
                    .Cast<IMajorRecordGetter>()
                    .ToList()
                    .AsObservableChangeSet()
                : new List<IMajorRecordGetter>()
                    .AsObservableChangeSet())
            .Switch()
            .ToObservableCollection(UnloadDisposable);
    }
}
