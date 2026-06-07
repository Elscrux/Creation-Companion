using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using FluentAvalonia.UI.Controls;
using NiflySharp;
using NifPlugin.Models;
using NifPlugin.ViewModels;
using Noggog;
using ReactiveUI;
using ReactiveUI.Avalonia;
namespace NifPlugin.Views;

public partial class NifView : ReactiveUserControl<NifVM> {
    public NifView() {
        InitializeComponent();
    }

    public NifView(NifVM vm) : this() {
        DataContext = vm;
    }

    private void Control_OnContextRequested(object? sender, ContextRequestedEventArgs e) {
        if (ViewModel is null) return;
        if (e.Source is not Control { DataContext: NifBlock block } control) return;

        List<MenuItem> menuItems = [];

        if (ViewModel.NifEditVertexColorService.HasVertexColor(block.NiObject)) {
            menuItems.AddRange([
                ShiftMenuItem("Edit Vertex Color", ViewModel.ShiftHue, ViewModel.ShiftSaturation, ViewModel.ShiftLightness),
            ]);
        }

        if (menuItems.Count == 0) return;

        var contextFlyout = new MenuFlyout { ItemsSource = menuItems };
        contextFlyout.ShowAt(control, true);

        MenuItem ShiftMenuItem(
            string header,
            Action<INiObject, double> shiftHueFunc,
            Action<INiObject, double> shiftSaturationFunc,
            Action<INiObject, double> shiftLightnessFunc) {
            return new MenuItem {
                Header = header,
                Command = ReactiveCommand.Create(async () => {
                    var hueSlider = new Slider {
                        Name = "HueSlider",
                        Value = 0,
                        Minimum = -360,
                        Maximum = 360,
                    };
                    var saturationSlider = new Slider {
                        Name = "SaturationSlider",
                        Value = 0,
                        Minimum = -255,
                        Maximum = 255,
                    };
                    var lightnessSlider = new Slider {
                        Name = "LightnessSlider",
                        Value = 0,
                        Minimum = -255,
                        Maximum = 255,
                    };

                    var stackPanel = new StackPanel {
                        Spacing = 4,
                        Children = {
                            new TextBlock {
                                [!TextBlock.TextProperty] = hueSlider
                                    .GetObservable(RangeBase.ValueProperty)
                                    .Select(value => $"Hue Shift ({value.Round()})")
                                    .ToBinding()
                            },
                            hueSlider,
                            new TextBlock {
                                [!TextBlock.TextProperty] = saturationSlider
                                    .GetObservable(RangeBase.ValueProperty)
                                    .Select(value => $"Saturation Shift ({value.Round()})")
                                    .ToBinding()
                            },
                            saturationSlider,
                            new TextBlock {
                                [!TextBlock.TextProperty] = lightnessSlider
                                    .GetObservable(RangeBase.ValueProperty)
                                    .Select(value => $"Lightness Shift ({value.Round()})")
                                    .ToBinding()
                            },
                            lightnessSlider,
                        }
                    };

                    var taskDialog = ViewModel.TaskDialogProvider.CreateTaskDialog(header, stackPanel, xamlRoot: this);
                    if (await taskDialog.ShowAsync() is FATaskDialogStandardResult.OK) {
                        shiftHueFunc(block.NiObject, hueSlider.Value);
                        shiftSaturationFunc(block.NiObject, saturationSlider.Value);
                        shiftLightnessFunc(block.NiObject, lightnessSlider.Value);
                    }
                }),
            };
        }
    }
}
