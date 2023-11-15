using System.Reactive.Linq;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Xaml.Interactivity;
using Noggog;
using ReactiveMarbles.ObservableEvents;
namespace CreationEditor.Avalonia.Behavior.TreeDataGrid;

public sealed class ScrollToSelection : Behavior<global::Avalonia.Controls.TreeDataGrid> {
    private readonly DisposableBucket _disposables = new();

    protected override void OnAttachedToVisualTree() {
        if (AssociatedObject is { RowSelection: {} selection }) {
            (selection as ITreeSelectionModel).Events().SelectionChanged
                .Select(_ => selection.SelectedItem)
                .NotNull()
                .ThrottleMedium()
                .Subscribe(selected => {
                    if (AssociatedObject.RowsPresenter?.Items is null) return;

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
