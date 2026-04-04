using System.Reactive;
using Avalonia.Controls;
using CreationEditor.Avalonia.ViewModels.Docking;
using FluentAvalonia.UI.Controls;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Avalonia.Models.Docking;

public interface IDockedItem : IDockObject, IEquatable<IDockedItem> {
    Guid Id { get; }

    Control Control { get; }

    new DockContainerVM DockParent { get; set; }

    bool IsSelected { get; set; }

    string? Header { get; set; }
    IconSource? IconSource { get; set; }

    double? Size { get; set; }

    bool CanClose { get; set; }
    ReactiveCommand<Unit, IObservable<IDockedItem>> Close { get; }

    DisposableCounterLock RemovalLock { get; }

    IObservable<IDockedItem> Closed { get; }
}
