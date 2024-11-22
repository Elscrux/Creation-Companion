using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Docking;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Docking;

public interface IDockedItem : IDockObject, IEquatable<IDockedItem> {
    Guid Id { get; }

    Control Control { get; }

    new DockContainerVM DockParent { get; set; }

    [Reactive] bool IsSelected { get; set; }

    [Reactive] string? Header { get; set; }
    [Reactive] IconSource? IconSource { get; set; }

    double? Size { get; set; }

    [Reactive] bool CanClose { get; set; }
    ReactiveCommand<Unit, IObservable<IDockedItem>> Close { get; }

    DisposableCounterLock RemovalLock { get; }

    IObservable<IDockedItem> Closed { get; }
}
