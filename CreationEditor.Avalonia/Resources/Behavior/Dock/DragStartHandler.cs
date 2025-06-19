using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Noggog;
namespace CreationEditor.Avalonia.Behavior;

public sealed class DragStartHandler(
    Func<object?, object?, PointerEventArgs, Task> dragCallback,
    double dragStartDistance = 10) {

    private Point _clickPosition;
    private bool _isPressingDown;

    private readonly Dictionary<Interactive, object> _elementIdentifiers = new();
    private readonly Dictionary<object, List<Interactive>> _identifierElements = new();

    public void Register(Interactive element, object? identifier = null) {
        // todo register also for list item boxes loaded later
        const RoutingStrategies routingStrategies = RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble;
        element.AddHandler(InputElement.PointerPressedEvent, Pressed, routingStrategies);
        element.AddHandler(InputElement.PointerMovedEvent, Moved, routingStrategies);
        element.AddHandler(InputElement.PointerReleasedEvent, Released, routingStrategies);

        if (identifier is not null) {
            _elementIdentifiers.TryAdd(element, identifier);
            var list = _identifierElements.GetOrAdd(identifier);
            list.Add(element);
        }
    }

    public void Unregister(Interactive element, object? identifier = null) {
        element.RemoveHandler(InputElement.PointerPressedEvent, Pressed);
        element.RemoveHandler(InputElement.PointerMovedEvent, Moved);
        element.RemoveHandler(InputElement.PointerReleasedEvent, Released);

        if (identifier is not null) {
            _elementIdentifiers.Remove(element);

            if (_identifierElements.TryGetValue(identifier, out var list)) {
                list.Remove(element);
            }
        }
    }

    public void UnregisterIdentifier(object identifier) {
        if (_identifierElements.Remove(identifier, out var elements)) {
            foreach (var element in elements) Unregister(element, identifier);
        }
    }

    public void Pressed(object? o, PointerPressedEventArgs e) {
        if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed) return;

        _isPressingDown = true;
        _clickPosition = e.GetPosition(null);
    }

    public Task Moved(object? o, PointerEventArgs e) {
        if (o is not Interactive interactive) return Task.CompletedTask;
        if (!_isPressingDown) return Task.CompletedTask;

        var distance = _clickPosition - e.GetPosition(null);
        if (Math.Abs(distance.X) < dragStartDistance && Math.Abs(distance.Y) < dragStartDistance) return Task.CompletedTask;

        _isPressingDown = false;

        _elementIdentifiers.TryGetValue(interactive, out var identifier);
        return dragCallback(o, identifier, e);
    }

    public void Released(object? o, PointerReleasedEventArgs e) {
        _isPressingDown = false;
    }
}
