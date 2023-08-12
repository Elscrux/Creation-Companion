using Avalonia.ReactiveUI;
using QueryPlugin.ViewModels;
namespace QueryPlugin.Views;

public partial class QueryPluginView : ReactiveUserControl<QueryPluginVM> {
    public QueryPluginView() {
        InitializeComponent();
    }

    public QueryPluginView(QueryPluginVM queryPluginVM) : this() {
        ViewModel = queryPluginVM;
    }
}
