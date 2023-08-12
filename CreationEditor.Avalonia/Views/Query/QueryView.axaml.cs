﻿using System.Reactive.Disposables;
using Avalonia.ReactiveUI;
using CreationEditor.Avalonia.ViewModels.Query;
using CreationEditor.Services.Query;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Query;

public partial class QueryView : ReactiveUserControl<QueryVM> {
    public QueryView() {
        InitializeComponent();

        this.WhenActivated(disposables => {
            this.WhenAnyValue(x => x.FromAutoCompleteBox.SelectedItem)
                .Subscribe(x => {
                    if (ViewModel == null) return;
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
