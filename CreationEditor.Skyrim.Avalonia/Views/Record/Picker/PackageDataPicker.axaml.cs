﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using CreationEditor.Avalonia.Views;
using CreationEditor.Skyrim.Avalonia.Resources.Constants;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Picker;

public sealed record PackageData(sbyte Index, string? Name, string Type, object? Value);

public partial class PackageDataPicker : ActivatableUserControl {
    public static readonly StyledProperty<IPackageGetter?> PackageProperty
        = AvaloniaProperty.Register<PackageDataPicker, IPackageGetter?>(nameof(Package));

    public static readonly StyledProperty<sbyte> PackageDataIndexProperty
        = AvaloniaProperty.Register<PackageDataPicker, sbyte>(nameof(PackageDataIndex));

    public static readonly StyledProperty<List<PackageData>?> DataProperty
        = AvaloniaProperty.Register<PackageDataPicker, List<PackageData>?>(nameof(Data));

    public static readonly StyledProperty<PackageData?> SelectedDataProperty
        = AvaloniaProperty.Register<PackageDataPicker, PackageData?>(nameof(SelectedData));

    public static readonly StyledProperty<ILinkCache?> LinkCacheProperty
        = AvaloniaProperty.Register<PackageDataPicker, ILinkCache?>(nameof(LinkCache));

    public static readonly StyledProperty<IEnumerable<Type>?> ScopedTypesProperty
        = AvaloniaProperty.Register<PackageDataPicker, IEnumerable<Type>?>(nameof(ScopedTypes));

    public sbyte PackageDataIndex {
        get => GetValue(PackageDataIndexProperty);
        set => SetValue(PackageDataIndexProperty, value);
    }

    public IPackageGetter? Package {
        get => GetValue(PackageProperty);
        set => SetValue(PackageProperty, value);
    }

    public PackageData? SelectedData {
        get => GetValue(SelectedDataProperty);
        set => SetValue(SelectedDataProperty, value);
    }

    public List<PackageData>? Data {
        get => GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public ILinkCache? LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public IEnumerable<Type>? ScopedTypes {
        get => GetValue(ScopedTypesProperty);
        set => SetValue(ScopedTypesProperty, value);
    }

    public PackageDataPicker() {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e) {
        base.OnAttachedToVisualTree(e);

        // Populate the data when the package changes
        this.WhenAnyValue(
                x => x.Package,
                x => x.LinkCache,
                x => x.ScopedTypes,
                (package, linkCache, scopedTypes) => (Package: package, LinkCache: linkCache, ScopedTypes: scopedTypes))
            .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
            .ObserveOnTaskpool()
            .Select(x => {
                if (x.Package == null || x.LinkCache == null) return null;

                var names = x.Package.GetNameFromTemplate(x.LinkCache);

                var types = x.ScopedTypes?.ToArray();

                return x.Package?.Data
                    .Where(d => {
                        if (types == null) return true;

                        var packageTypes = d.Value switch {
                            IPackageDataBoolGetter => RecordTypeConstants.PackageDataNumericTypes,
                            IPackageDataFloatGetter => RecordTypeConstants.PackageDataNumericTypes,
                            IPackageDataIntGetter => RecordTypeConstants.PackageDataNumericTypes,
                            IPackageDataLocationGetter => RecordTypeConstants.LocationTypes,
                            IPackageDataTargetGetter => RecordTypeConstants.AllPlacedInterfaceTypes,
                            IPackageDataObjectListGetter => RecordTypeConstants.AllPlacedInterfaceTypes,
                            _ => null
                        };
                        return packageTypes != null && packageTypes.AnyInheritsFromAny(types);
                    })
                    .Select(d => {
                        var valueName = d.Value.Name ?? names[d.Key];
                        return d.Value switch {
                            IPackageDataBoolGetter boolData => new PackageData(d.Key, valueName, "Bool", boolData.Data),
                            IPackageDataFloatGetter floatData => new PackageData(d.Key, valueName, "Float", floatData.Data),
                            IPackageDataIntGetter intData => new PackageData(d.Key, valueName, "Int", intData.Data),
                            IPackageDataLocationGetter locationData => new PackageData(d.Key, valueName, "Location",
                                locationData.Location.Target switch {
                                    ILocationCellGetter cell => $"Cell: {cell.Link.GetEditorID(x.LinkCache)}",
                                    ILocationFallbackGetter fallback => fallback.Type,
                                    ILocationKeywordGetter keyword => $"Keyword: {keyword.Link.GetEditorID(x.LinkCache)}",
                                    ILocationObjectIdGetter objectId => $"Object: {objectId.Link.GetEditorID(x.LinkCache)}",
                                    ILocationObjectTypeGetter objectType => $"Type: {objectType.Type}",
                                    ILocationTargetGetter target => $"Target: {target.Link.GetSelfOrBaseEditorID(x.LinkCache)}",
                                    _ => throw new ArgumentOutOfRangeException()
                                }),
                            IPackageDataObjectListGetter objectListData => new PackageData(d.Key, valueName, "Object List", $"Radius: {objectListData.Data}"),
                            IPackageDataTargetGetter targetData => new PackageData(d.Key, valueName, "Target", targetData.Target.CountOrDistance),
                            IPackageDataTopicGetter topicData => new PackageData(d.Key, valueName, "Topic", topicData.Topics.FirstOrDefault()),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                    }).ToList();
            })
            .NotNull()
            .ObserveOnGui()
            .Subscribe(list => {
                // Force keep data index which is reset when the items change
                var c = PackageDataIndex;
                PackageDataIndex = -1;
                Data = list;
                PackageDataIndex = (sbyte) (c >= 0 && c < Data.Count ? c : 0);
            })
            .DisposeWith(ActivatedDisposable);

        // Set the selected data when index or package data changes
        this.WhenAnyValue(
                x => x.PackageDataIndex,
                x => x.Data,
                (index, data) => (Index: index, Data: data))
            .Where(x => x.Data != null && x.Data.Any())
            .Subscribe(x => {
                if (x.Data == null) {
                    SelectedData = null;
                } else {
                    var alias = x.Data.FirstOrDefault(d => d.Index == x.Index);
                    SelectedData = alias ?? x.Data.FirstOrDefault();
                }
            })
            .DisposeWith(ActivatedDisposable);

    }
}
