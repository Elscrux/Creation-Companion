using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using CreationEditor.Avalonia;
using CreationEditor.Skyrim.Avalonia.Resources.Converter;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;
using ReactiveUI;

namespace CreationEditor.Skyrim.Avalonia.Views.Record.Picker;

public partial class PlacedPickerButton : UserControl {
    public static readonly StyledProperty<IFormLinkGetter?> PlacedProperty
        = AvaloniaProperty.Register<PlacedPickerButton, IFormLinkGetter?>(nameof(Placed));

    public static readonly StyledProperty<FormKey> CellProperty
        = AvaloniaProperty.Register<PlacedPickerButton, FormKey>(nameof(Cell));

    public static readonly StyledProperty<ILinkCache?> LinkCacheProperty
        = AvaloniaProperty.Register<PlacedPickerButton, ILinkCache?>(nameof(LinkCache));

    public static readonly StyledProperty<bool> IsOpenProperty
        = AvaloniaProperty.Register<PlacedPickerButton, bool>(nameof(IsOpen));

    public static readonly StyledProperty<IObservable<string?>> ButtonTextProperty
        = AvaloniaProperty.Register<PlacedPickerButton, IObservable<string?>>(nameof(ButtonText));

    public IFormLinkGetter? Placed {
        get => GetValue(PlacedProperty);
        set => SetValue(PlacedProperty, value);
    }

    public FormKey Cell {
        get => GetValue(CellProperty);
        set => SetValue(CellProperty, value);
    }

    public ILinkCache? LinkCache {
        get => GetValue(LinkCacheProperty);
        set => SetValue(LinkCacheProperty, value);
    }

    public bool IsOpen {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public IObservable<string?> ButtonText {
        get => GetValue(ButtonTextProperty);
        set => SetValue(ButtonTextProperty, value);
    }

    public PlacedPickerButton() {
        InitializeComponent();

        ButtonText = this.WhenAnyValue(x => x.IsOpen)
            .Where(isOpen => isOpen == false)
            .Select(_ => {
                if (LinkCache != null && Placed != null) {
                    if (LinkCache.TryResolve<ICellGetter>(Cell, out var cell)) {
                        var placed = cell.Temporary.Concat(cell.Persistent).FirstOrDefault(placed => placed.FormKey == Placed.FormKey);
                        if (placed != null) {
                            if (placed.EditorID == null) {
                                return PlacedConverters.ToName.Convert(placed, LinkCache) as string ?? placed.FormKey.ToString();
                            }

                            return placed.EditorID;
                        }
                    }
                }
                return "Select";
            });
    }
}
