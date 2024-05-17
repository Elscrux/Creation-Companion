using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using CreationEditor.Avalonia;
using CreationEditor.Avalonia.Views;
using CreationEditor.Skyrim.Avalonia.Resources.Converter;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Picker;

public partial class PlacedPickerButton : ActivatableUserControl {
    public static readonly StyledProperty<IFormLinkGetter?> PlacedProperty
        = AvaloniaProperty.Register<PlacedPickerButton, IFormLinkGetter?>(nameof(Placed));

    public static readonly StyledProperty<FormKey> CellProperty
        = PlacedPicker.CellProperty.AddOwner<PlacedPickerButton>();

    public static readonly StyledProperty<IEnumerable<Type>> ScopedTypesProperty
        = PlacedPicker.ScopedTypesProperty.AddOwner<PlacedPickerButton>();

    public static readonly StyledProperty<ILinkCache?> LinkCacheProperty
        = PlacedPicker.LinkCacheProperty.AddOwner<PlacedPickerButton>();

    public static readonly StyledProperty<Func<IPlacedGetter, bool>> FilterProperty
        = PlacedPicker.FilterProperty.AddOwner<PlacedPickerButton>();

    public static readonly StyledProperty<bool> IsOpenProperty
        = AvaloniaProperty.Register<PlacedPickerButton, bool>(nameof(IsOpen));

    public static readonly StyledProperty<IObservable<string?>?> ButtonTextProperty
        = AvaloniaProperty.Register<PlacedPickerButton, IObservable<string?>?>(nameof(ButtonText));

    public IFormLinkGetter? Placed {
        get => GetValue(PlacedProperty);
        set => SetValue(PlacedProperty, value);
    }

    public FormKey Cell {
        get => GetValue(CellProperty);
        set => SetValue(CellProperty, value);
    }

    public IEnumerable<Type> ScopedTypes {
        get => GetValue(ScopedTypesProperty);
        set => SetValue(ScopedTypesProperty, value);
    }

    public ILinkCache? LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public Func<IPlacedGetter, bool> Filter {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }

    public bool IsOpen {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public IObservable<string?>? ButtonText {
        get => GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public PlacedPickerButton() {
        InitializeComponent();
    }

    protected override void WhenActivated() {
        var placedChanged = PlacedPicker.PlacedChanged
            .Merge(this.WhenAnyValue(x => x.Placed)
                .Select(x => x?.FormKey ?? FormKey.Null));

        var pickerUpdated = this.WhenAnyValue(
                x => x.LinkCache,
                x => x.Cell,
                (linkCache, cell) => (LinkCache: linkCache, Cell: cell))
            .CombineLatest(placedChanged, (x, placed) => (x.LinkCache, x.Cell, Placed: placed));

        // Update the cell when the cell is not set and placed is set
        pickerUpdated
            .ObserveOnTaskpool()
            .Select(x => {
                if (x.Placed.IsNull || !x.Cell.IsNull || x.LinkCache is null) return FormKey.Null;
                if (!x.LinkCache.TryResolveSimpleContext<IPlacedGetter>(x.Placed, out var placed)) return FormKey.Null;
                if (placed.Parent?.Record is ICellGetter cell) return cell.FormKey;

                return FormKey.Null;
            })
            .Where(x => x != FormKey.Null)
            .ObserveOnGui()
            .Subscribe(cell => Cell = cell)
            .DisposeWith(ActivatedDisposable);

        // Update button text to the latest placed record
        ButtonText = pickerUpdated
            .ObserveOnTaskpool()
            .Select(x => {
                const string defaultText = "Select";

                if (x is not { LinkCache: not null }) return defaultText;
                if (x.Placed.IsNull) return defaultText;

                // In case the cell can't be resolved, return null to show spinner
                if (!x.LinkCache.TryResolve<ICellGetter>(x.Cell, out var cell)) return null;

                var placed = cell.GetAllPlaced(x.LinkCache).FirstOrDefault(placed => placed.FormKey == x.Placed);
                if (placed is null) return defaultText;

                var placedEditorID = PlacedConverters.ToName.Convert(placed, x.LinkCache) as string;

                var cellEditorID = cell.EditorID;
                return cellEditorID is null
                    ? placedEditorID
                    : $"{cellEditorID}: {placedEditorID}";
            });
    }
}
