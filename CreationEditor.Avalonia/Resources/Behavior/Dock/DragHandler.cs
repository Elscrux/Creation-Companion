using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Noggog;
namespace CreationEditor.Avalonia.Behavior;

public sealed class DragHandler {
    private Point _clickPosition;
    private bool _isPressingDown;

    private readonly double _dragStartDistance;
    private readonly Action<object?, object?, PointerEventArgs> _dragCallback;

    private readonly Dictionary<Interactive, object> _elementIdentifiers = new();
    private readonly Dictionary<object, List<Interactive>> _identifierElements = new();

    public DragHandler(Action<object?, object?, PointerEventArgs> dragCallback, double dragStartDistance = 10) {
        _dragStartDistance = dragStartDistance;
        _dragCallback = dragCallback;
    }

    public void Register(Interactive element, object? identifier = null) {
        element.AddHandler(InputElement.PointerPressedEvent, Pressed, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        element.AddHandler(InputElement.PointerMovedEvent, Moved, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
        element.AddHandler(InputElement.PointerReleasedEvent, Released, RoutingStrategies.Direct | RoutingStrategies.Tunnel | RoutingStrategies.Bubble);

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
        if (_identifierElements.TryGetValue(identifier, out var elements)) {
            _identifierElements.Remove(identifier);
            foreach (var element in elements) Unregister(element, identifier);
        }
    }

    public void Pressed(object? o, PointerPressedEventArgs e) {
        if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed) return;

        _isPressingDown = true;
        _clickPosition = e.GetPosition(null);
    }

    public void Moved(object? o, PointerEventArgs e) {
        if (o is not Interactive interactive) return;
        if (!_isPressingDown) return;

        var distance = _clickPosition - e.GetPosition(null);
        if (Math.Abs(distance.X) < _dragStartDistance && Math.Abs(distance.Y) < _dragStartDistance) return;

        _isPressingDown = false;

        _elementIdentifiers.TryGetValue(interactive, out var identifier);
        _dragCallback(o, identifier, e);
    }

    public void Released(object? o, PointerReleasedEventArgs e) {
        _isPressingDown = false;
    }
}
