using System.Reactive.Disposables;
using Avalonia.Data.Converters;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.Converter;
using CreationEditor.Avalonia.ViewModels.Query;
using CreationEditor.Services.Query.From;
using CreationEditor.Services.Query.Select;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Query;

public partial class QueryView : ReactiveUserControl<QueryVM> {
    public QueryView() {
        InitializeComponent();

        this.WhenActivated(disposables => {
            this.WhenAnyValue(x => x.FromAutoCompleteBox.SelectedItem)
                .Subscribe(x => {
                    if (ViewModel is null) return;
                    if (x is not QueryFromItem queryFromItem) return;

                    ViewModel.QueryRunner.QueryFrom.SelectedItem = queryFromItem;
                })
                .DisposeWith(disposables);
        });
    }

    public QueryView(QueryVM vm) : this() {
        DataContext = vm;
    }
}
