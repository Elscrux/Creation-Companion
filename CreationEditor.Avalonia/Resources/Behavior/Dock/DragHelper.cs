using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
namespace CreationEditor.Avalonia.Behavior;

public sealed class DragHandler {
    private Point _clickPosition;
    private bool _isPressingDown;

    private readonly double _dragStartDistance;
    private readonly Action<object?, object?, PointerEventArgs> _dragCallback;

    private readonly Dictionary<object, object> _elementIdentifiers = new();

    public DragHandler(Action<object?, object?, PointerEventArgs> dragCallback, double dragStartDistance = 10) {
        _dragStartDistance = dragStartDistance;
        _dragCallback = dragCallback;
    }

    public void Register(Interactive element, object? identifier = null) {
        element.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        element.AddHandler(InputElement.PointerMovedEvent, Moved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        element.AddHandler(InputElement.PointerReleasedEvent, Released, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

        if (identifier != null) _elementIdentifiers.TryAdd(element, identifier);
    }

    public void Unregister(Interactive associatedObject) {
        associatedObject.RemoveHandler(InputElement.PointerPressedEvent, Pressed);
        associatedObject.RemoveHandler(InputElement.PointerMovedEvent, Moved);
        associatedObject.RemoveHandler(InputElement.PointerReleasedEvent, Released);
    }

    public void Pressed(object? o, PointerPressedEventArgs e) {
        if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed) return;

        _isPressingDown = true;
        _clickPosition = e.GetPosition(null);
    }

    public void Moved(object? o, PointerEventArgs e) {
        if (!_isPressingDown) return;

        var distance = _clickPosition - e.GetPosition(null);
        if (Math.Abs(distance.X) < _dragStartDistance && Math.Abs(distance.Y) < _dragStartDistance) return;

        _isPressingDown = false;

        if (o != null && _elementIdentifiers.TryGetValue(o, out var identifier)) {
            _dragCallback(o, identifier, e);
        } else {
            _dragCallback(o, null, e);
        }
    }

    public void Released(object? o, PointerReleasedEventArgs e) {
        _isPressingDown = false;
    }
}
