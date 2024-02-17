using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
namespace CreationEditor.Avalonia.Behavior;

public sealed class DataGridTextSearchBehavior : Behavior<DataGrid> {
    public static readonly StyledProperty<Func<string, object?>> TextSearchValueSelectorProperty
        = AvaloniaProperty.Register<AvaloniaObject, Func<string, object?>>(nameof(TextSearchValueSelector));

    public Func<string, object?> TextSearchValueSelector {
        get => GetValue(TextSearchValueSelectorProperty);
        set => SetValue(TextSearchValueSelectorProperty, value);
    }

    protected override void OnAttached() {
        base.OnAttached();

        AssociatedObject?.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
    }

    protected override void OnDetaching() {
        base.OnDetaching();

        AssociatedObject?.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
    }

    private const long ThrottleTime = 5000000; // 500ms
    private string _currentSearch = string.Empty;
    private long _lastTimestamp = long.MinValue;

    private void OnKeyDown(object? sender, KeyEventArgs e) {
        if (e.KeyModifiers != KeyModifiers.None || AssociatedObject is null || AssociatedObject.Columns.Count == 0) return;

        var currentTimestamp = Stopwatch.GetTimestamp();
        if (currentTimestamp - _lastTimestamp > ThrottleTime) _currentSearch = string.Empty;
        _lastTimestamp = currentTimestamp;

        _currentSearch += e.KeySymbol;

        var target = TextSearchValueSelector?.Invoke(_currentSearch);
        if (target is null) return;

        AssociatedObject.SelectedItem = target;
        AssociatedObject.ScrollIntoView(target, AssociatedObject.Columns[0]);
    }
}
