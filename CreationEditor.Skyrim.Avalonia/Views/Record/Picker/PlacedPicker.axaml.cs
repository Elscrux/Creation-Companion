using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using CreationEditor.Avalonia;
using CreationEditor.Avalonia.Views;
using CreationEditor.Avalonia.Views.Record.Picker;
using CreationEditor.Skyrim.Avalonia.Resources.Constants;
using CreationEditor.Skyrim.Avalonia.Resources.Converter;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Picker;

public partial class PlacedPicker : ActivatableUserControl {
    public static readonly StyledProperty<ILinkCache?> LinkCacheProperty
        = AvaloniaProperty.Register<PlacedPicker, ILinkCache?>(nameof(LinkCache));

    public static readonly StyledProperty<FormLink<IMajorRecordGetter>> PlacedLinkProperty
        = AvaloniaProperty.Register<PlacedPicker, FormLink<IMajorRecordGetter>>(nameof(PlacedLink), new FormLink<IMajorRecordGetter>());

    public static readonly StyledProperty<FormKey> PlacedFormKeyProperty
        = AvaloniaProperty.Register<PlacedPicker, FormKey>(nameof(PlacedFormKey));

    public static readonly StyledProperty<FormKey> CellProperty
        = AvaloniaProperty.Register<PlacedPicker, FormKey>(nameof(Cell));

    public static readonly StyledProperty<IEnumerable<Type>> ScopedTypesProperty
        = AvaloniaProperty.Register<PlacedPicker, IEnumerable<Type>>(nameof(ScopedTypes), RecordTypeConstants.PlacedTypes);

    public static readonly StyledProperty<ReadOnlyObservableCollection<IMajorRecordGetter>> PlacedRecordsProperty
        = AvaloniaProperty.Register<PlacedPicker, ReadOnlyObservableCollection<IMajorRecordGetter>>(nameof(PlacedRecords));

    public static readonly StyledProperty<IPlacedGetter?> ResolvedPlacedProperty
        = AvaloniaProperty.Register<PlacedPicker, IPlacedGetter?>(nameof(ResolvedPlaced));

    public static readonly StyledProperty<Func<IMajorRecordIdentifier, ILinkCache?, string?>> PlacedNameSelectorProperty
        = AvaloniaProperty.Register<PlacedPicker, Func<IMajorRecordIdentifier, ILinkCache?, string?>>(nameof(PlacedNameSelector));

    public static readonly StyledProperty<Func<IMajorRecordIdentifier, ILinkCache?, string?>> CellNameSelectorProperty
        = AvaloniaProperty.Register<PlacedPicker, Func<IMajorRecordIdentifier, ILinkCache?, string?>>(nameof(CellNameSelector));

    public static readonly StyledProperty<Func<IPlacedGetter, bool>> FilterProperty
        = AvaloniaProperty.Register<PlacedPicker, Func<IPlacedGetter, bool>>(nameof(Filter), _ => true);

    public static readonly StyledProperty<IObservable<FormKey>> PlacedChangedProperty
        = AFormKeyPicker.FormKeyChangedProperty.AddOwner<PlacedPicker>();

    public ILinkCache? LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public FormLink<IMajorRecordGetter> PlacedLink {
        get => GetValue(PlacedLinkProperty);
        set => SetValue(PlacedLinkProperty, value);
    }

    public FormKey PlacedFormKey {
        get => GetValue(PlacedFormKeyProperty);
        set => SetValue(PlacedFormKeyProperty, value);
    }

    public FormKey Cell {
        get => GetValue(CellProperty);
        set => SetValue(CellProperty, value);
    }

    public IEnumerable<Type> ScopedTypes {
        get => GetValue(ScopedTypesProperty);
        set => SetValue(ScopedTypesProperty, value);
    }

    public ReadOnlyObservableCollection<IMajorRecordGetter> PlacedRecords {
        get => GetValue(PlacedRecordsProperty);
        set => SetValue(PlacedRecordsProperty, value);
    }

    public IPlacedGetter? ResolvedPlaced {
        get => GetValue(ResolvedPlacedProperty);
        set => SetValue(ResolvedPlacedProperty, value);
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

    public IObservable<FormKey> PlacedChanged {
        get => GetValue(PlacedChangedProperty);
        set => SetValue(PlacedChangedProperty, value);
    }

    public PlacedPicker() {
        InitializeComponent();

        PlacedNameSelector = (identifier, linkCache) => linkCache is null
            ? null
            : identifier is IPlacedGetter placed
                ? placed.GetSelfOrBaseEditorID(linkCache)
                : linkCache.TryResolve<IPlacedGetter>(identifier.FormKey, out var referencedPlaced)
                    ? referencedPlaced.GetSelfOrBaseEditorID(linkCache)
                    : null;

        CellNameSelector = (identifier, linkCache) => linkCache is not null && linkCache.TryResolve<ICellGetter>(identifier.FormKey, out var cell)
            ? CellConverters.ToName.Convert(cell, linkCache) as string
            : null;
    }

    protected override void WhenActivated() {
        this.WhenAnyValue(x => x.PlacedFormKey)
            .Subscribe(placed => PlacedLink.SetTo(placed))
            .DisposeWith(ActivatedDisposable);

        // Initialize cell based on placed on load - after cell is set once, it will be set by the picker itself
        this.WhenAnyValue(
                x => x.LinkCache,
                x => x.PlacedFormKey,
                (linkCache, placed) => (LinkCache: linkCache, Placed: placed))
            .Where(x => x.LinkCache is not null && x.Placed != FormKey.Null)
            .Take(1)
            .ObserveOnTaskpool()
            .Select(x => {
                if (x.LinkCache!.TryResolveSimpleContext<IPlacedGetter>(x.Placed, out var placed)) {
                    if (placed.Parent?.Record is ICellGetter cell) {
                        return cell.FormKey;
                    }
                }

                return FormKey.Null;
            })
            .ObserveOnGui()
            .Subscribe(x => Cell = x)
            .DisposeWith(ActivatedDisposable);

        // Set placed to null when the cell is deselected
        this.WhenAnyValue(x => x.Cell)
            .Skip(1)
            .Buffer(2, 1)
            .Subscribe(x => {
                if (x is [var prev, var cur] && prev.IsNull != cur.IsNull) {
                    PlacedFormKey = FormKey.Null;
                }
            })
            .DisposeWith(ActivatedDisposable);

        // Refresh list of available placed records when cell changes
        PlacedRecords = this.WhenAnyValue(
                x => x.Cell,
                x => x.Filter,
                x => x.LinkCache,
                (cell, filter, linkCache) => (Cell: cell, Filter: filter, LinkCache: linkCache))
            .DistinctUntilChanged()
            .ObserveOnTaskpool()
            .Select(x => x.LinkCache is not null && x.LinkCache.TryResolve<ICellGetter>(x.Cell, out var record)
                ? record.Temporary
                    .Concat(record.Persistent)
                    .Where(x.Filter)
                    .Select(placed => (IMajorRecordGetter) placed)
                    .ToList()
                    .AsObservableChangeSet()
                : new List<IMajorRecordGetter>()
                    .AsObservableChangeSet())
            .Switch()
            .ToObservableCollection(ActivatedDisposable);

        this.WhenAnyValue(
                x => x.Cell,
                x => x.Filter,
                x => x.LinkCache,
                (cell, filter, linkCache) => (Cell: cell, Filter: filter, LinkCache: linkCache))
            .DistinctUntilChanged()
            .UpdateWhenCollectionChanges(PlacedRecords);

        // Update resolved placed when placed or placed records change
        this.WhenAnyValue(
                x => x.PlacedFormKey,
                x => x.PlacedRecords,
                (placed, placedRecords) => (Placed: placed, PlacedRecords: placedRecords))
            .DistinctUntilChanged()
            .UpdateWhenCollectionChanges(PlacedRecords)
            .ObserveOnTaskpool()
            .Select(x => new List<IMajorRecordGetter>(x.PlacedRecords).FirstOrDefault(y => y.FormKey == x.Placed) as IPlacedGetter)
            .ObserveOnGui()
            .Subscribe(placed => ResolvedPlaced = placed)
            .DisposeWith(ActivatedDisposable);
    }
}
