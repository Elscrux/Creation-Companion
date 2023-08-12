using Avalonia.ReactiveUI;
using QueryPlugin.ViewModels;
namespace QueryPlugin.Views; 

public partial class QueryColumn : ReactiveUserControl<QueryColumnVM> {
    public QueryColumn() {
        InitializeComponent();
    }
    
    public QueryColumn(QueryColumnVM vm) : this() {
        DataContext = vm;
    }
}
