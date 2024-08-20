using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Avalonia;
using CreationEditor.Avalonia.Views;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Picker;

public partial class TargetPicker : ActivatableUserControl {
    public static readonly StyledProperty<IObservable<string>> ButtonTextProperty
        = AvaloniaProperty.Register<TargetPicker, IObservable<string>>(nameof(ButtonText));

    public static readonly StyledProperty<bool> IsOpenProperty
        = AvaloniaProperty.Register<TargetPicker, bool>(nameof(IsOpen));

    public static readonly StyledProperty<IList<TargetPickerType>> TypesProperty
        = AvaloniaProperty.Register<TargetPicker, IList<TargetPickerType>>(nameof(Types));

    public static readonly StyledProperty<TargetPickerType> SelectedTypeProperty
        = AvaloniaProperty.Register<TargetPicker, TargetPickerType>(nameof(SelectedType));

    public static readonly StyledProperty<ALocationTarget> TargetProperty
        = AvaloniaProperty.Register<TargetPicker, ALocationTarget>(nameof(Target));

    public static readonly StyledProperty<ILinkCache> LinkCacheProperty
        = AvaloniaProperty.Register<TargetPicker, ILinkCache>(nameof(LinkCache));

    public static readonly StyledProperty<FormKey> CellProperty
        = AvaloniaProperty.Register<TargetPicker, FormKey>(nameof(Cell));

    private static readonly StyledProperty<FormKey> ReferenceProperty
        = AvaloniaProperty.Register<TargetPicker, FormKey>(nameof(Reference));

    public static readonly StyledProperty<FormKey> KeywordProperty
        = AvaloniaProperty.Register<TargetPicker, FormKey>(nameof(Keyword));

    public static readonly StyledProperty<FormKey> ObjectProperty
        = AvaloniaProperty.Register<TargetPicker, FormKey>(nameof(Object));

    public static readonly StyledProperty<TargetObjectType> ObjectTypeProperty
        = AvaloniaProperty.Register<TargetPicker, TargetObjectType>(nameof(ObjectType));

    public IObservable<string> ButtonText {
        get => GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public bool IsOpen {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public IList<TargetPickerType> Types {
        get => GetValue(TypesProperty);
        set => SetValue(TypesProperty, value);
    }

    public TargetPickerType SelectedType {
        get => GetValue(SelectedTypeProperty);
        set => SetValue(SelectedTypeProperty, value);
    }

    public ALocationTarget Target {
        get => GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    public ILinkCache LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public FormKey Cell {
        get => GetValue(CellProperty);
        set => SetValue(CellProperty, value);
    }

    public FormKey Reference {
        get => GetValue(ReferenceProperty);
        set => SetValue(ReferenceProperty, value);
    }

    public FormKey Keyword {
        get => GetValue(KeywordProperty);
        set => SetValue(KeywordProperty, value);
    }

    public FormKey Object {
        get => GetValue(ObjectProperty);
        set => SetValue(ObjectProperty, value);
    }

    public TargetObjectType ObjectType {
        get => GetValue(ObjectTypeProperty);
        set => SetValue(ObjectTypeProperty, value);
    }

    public TargetPicker() {
        InitializeComponent();
    }

    private static string GetEditorOrNone<T>(ILinkCache linkCache, FormKey formKey)
        where T : class, IMajorRecordQueryableGetter, IMajorRecordIdentifier {
        return linkCache.TryResolve<T>(formKey, out var record)
            ? record.EditorID ?? "No EditorID"
            : "None";
    }

    protected override void WhenActivated() {
        // Initialize the target values
        this.WhenAnyValue(x => x.Target)
            .Take(1)
            .Subscribe(target => {
                SelectedType = TargetToType(target);
                switch (target) {
                    case LocationCell cell:
                        Cell = cell.Link.FormKey;
                        break;
                    case LocationKeyword keyword:
                        Keyword = keyword.Link.FormKey;
                        break;
                    case LocationObjectId objectId:
                        Object = objectId.Link.FormKey;
                        break;
                    case LocationObjectType objectType:
                        ObjectType = objectType.Type;
                        break;
                    case LocationTarget locationTarget:
                        Reference = locationTarget.Link.FormKey;
                        break;
                }
            })
            .DisposeWith(ActivatedDisposable);

        // Update the button text when the target changes
        ButtonText = this.WhenAnyValue(
                x => x.LinkCache,
                x => x.SelectedType,
                x => x.Cell,
                x => x.Reference,
                x => x.PlacedPicker.ResolvedPlaced,
                x => x.Keyword,
                x => x.Object,
                x => x.ObjectType,
                (linkCache, type, cell, reference, placed, keyword, obj, objectType) => (
                    LinkCache: linkCache,
                    Type: type,
                    Cell: cell,
                    Reference: reference,
                    Placed: placed,
                    Keyword: keyword,
                    Object: obj,
                    ObjectType: objectType))
            .ObserveOnTaskpool()
            .Select(x => x.Type switch {
                TargetPickerType.Cell => $"Cell: {GetEditorOrNone<ICellGetter>(x.LinkCache, x.Cell)}",
                TargetPickerType.LinkedRef => $"Linked Reference: {GetEditorOrNone<IKeywordGetter>(x.LinkCache, x.Keyword)}",
                TargetPickerType.Object => $"Object: {GetEditorOrNone<IObjectIdGetter>(x.LinkCache, x.Object)}",
                TargetPickerType.ObjectType => $"Type: {x.ObjectType}",
                TargetPickerType.Self => "Self",
                TargetPickerType.PackageStart => "Package Start",
                TargetPickerType.EditorLocation => "Editor Location",
                TargetPickerType.Reference => "Reference: " + (x.Placed?.GetSelfOrBaseEditorID(x.LinkCache)
                 ?? x.Reference.ToLinkGetter<IPlacedGetter>().GetSelfOrBaseEditorID(x.LinkCache)
                 ?? "None"),
                _ => throw new ArgumentOutOfRangeException(nameof(x), x, null),
            });

        // Update the target to the selected type when the dialog closes
        this.WhenAnyValue(x => x.IsOpen)
            .WhereFalse()
            .CombineLatest(this.WhenAnyValue(x => x.SelectedType), (_, type) => type)
            .Subscribe(type => {
                Target = type switch {
                    TargetPickerType.Cell => new LocationCell { Link = new FormLink<ICellGetter>(Cell) },
                    TargetPickerType.LinkedRef => new LocationKeyword { Link = new FormLink<IKeywordGetter>(Keyword) },
                    TargetPickerType.Object => new LocationObjectId { Link = new FormLink<IObjectIdGetter>(Object) },
                    TargetPickerType.ObjectType => new LocationObjectType { Type = ObjectType },
                    TargetPickerType.Self => new LocationFallback { Type = LocationTargetRadius.LocationType.NearSelf },
                    TargetPickerType.PackageStart => new LocationFallback { Type = LocationTargetRadius.LocationType.NearPackageStart },
                    TargetPickerType.EditorLocation => new LocationFallback { Type = LocationTargetRadius.LocationType.NearEditorLocation },
                    TargetPickerType.Reference => new LocationTarget { Link = new FormLink<IPlacedGetter>(Reference) },
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
                };
            })
            .DisposeWith(ActivatedDisposable);
    }

    private static TargetPickerType TargetToType(ALocationTarget target) {
        return target switch {
            LocationCell => TargetPickerType.Cell,
            LocationKeyword => TargetPickerType.LinkedRef,
            LocationObjectId => TargetPickerType.Object,
            LocationObjectType => TargetPickerType.ObjectType,
            LocationTarget => TargetPickerType.Reference,
            LocationFallback locationFallback => locationFallback.Type switch {
                LocationTargetRadius.LocationType.AliasForReference => TargetPickerType.ReferenceAlias,
                LocationTargetRadius.LocationType.AliasForLocation => TargetPickerType.LocationAlias,
                LocationTargetRadius.LocationType.AtPackageLocation => TargetPickerType.PackageLocation,
                LocationTargetRadius.LocationType.NearEditorLocation => TargetPickerType.EditorLocation,
                LocationTargetRadius.LocationType.NearPackageStart => TargetPickerType.PackageStart,
                LocationTargetRadius.LocationType.NearSelf => TargetPickerType.Self,
                LocationTargetRadius.LocationType.NearReference => TargetPickerType.Reference,
                LocationTargetRadius.LocationType.InCell => TargetPickerType.Cell,
                LocationTargetRadius.LocationType.ObjectID => TargetPickerType.Object,
                LocationTargetRadius.LocationType.ObjectType => TargetPickerType.ObjectType,
                LocationTargetRadius.LocationType.LinkedReference => TargetPickerType.LinkedRef,
                _ => throw new InvalidOperationException(),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(target)),

        };
    }
}
