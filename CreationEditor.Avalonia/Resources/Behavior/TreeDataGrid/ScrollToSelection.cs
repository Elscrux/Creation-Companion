using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Xaml.Interactivity;
using CreationEditor.Avalonia.Models.Asset;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Behavior.TreeDataGrid;

public sealed class ScrollToSelection : Behavior<global::Avalonia.Controls.TreeDataGrid> {
    private readonly IDisposableDropoff _disposables = new DisposableBucket();

    protected override void OnAttachedToVisualTree() {
        if (AssociatedObject is { RowSelection: {} selection }) {
            Observable.FromEventPattern<EventHandler<TreeSelectionModelSelectionChangedEventArgs>, TreeSelectionModelSelectionChangedEventArgs>(
                    h => selection.SelectionChanged += h,
                    h => selection.SelectionChanged -= h)
                .Select(_ => selection.SelectedItem)
                .NotNull()
                .Throttle(TimeSpan.FromMilliseconds(300), RxApp.MainThreadScheduler)
                .Subscribe(selected => {
                    if (AssociatedObject.RowsPresenter?.Items == null) return;

                    var indexOf = AssociatedObject.RowsPresenter.Items.FindIndex<IRow, string>(row => ReferenceEquals(row.Model, selected));
                    if (indexOf == -1) return;

                    AssociatedObject.RowsPresenter.BringIntoView(indexOf);
                })
                .DisposeWith(_disposables);
        }
    }

    protected override void OnDetachedFromVisualTree() {
        _disposables.Dispose();
    }
}
