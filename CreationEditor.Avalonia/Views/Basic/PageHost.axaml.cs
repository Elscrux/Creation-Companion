using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views.Basic;

public partial class PageHost : ActivatableUserControl {
    public static readonly StyledProperty<ObservableCollection<PageData>?> PagesProperty
        = AvaloniaProperty.Register<PageHost, ObservableCollection<PageData>?>(nameof(Pages));

    public ObservableCollection<PageData>? Pages {
        get => GetValue(PagesProperty);
        set => SetValue(PagesProperty, value);
    }

    public static readonly StyledProperty<bool> HasFinalButtonProperty
        = AvaloniaProperty.Register<PageHost, bool>(nameof(HasFinalButton));

    public bool HasFinalButton {
        get => GetValue(HasFinalButtonProperty);
        set => SetValue(HasFinalButtonProperty, value);
    }

    public PageHost() {
        InitializeComponent();

        Pages = new ObservableCollectionExtended<PageData>();
    }

    private const string DefaultNextButtonName = "Next";

    protected override void WhenActivated() {
        base.WhenActivated();

        var tabIndexChanged = PageControl.GetObservable(TabIndexProperty);
        var tabAndPagesChanged = tabIndexChanged.CombineLatest(this.WhenAnyValue(x => x.Pages), (index, pages) => (Index: index, Pages: pages));

        var canGoForward = tabAndPagesChanged.CombineLatest(
                this.WhenAnyValue(x => x.HasFinalButton),
                (x, hasFinalButton) => (x.Index, x.Pages, HasFinalButton: hasFinalButton))
            .Select(x => {
                if (x.Pages is null || (x.HasFinalButton ? x.Index : x.Index + 1) >= x.Pages.Count) return Observable.Return(false);
                if (x.Pages[x.Index] is { OnNext: {} command }) return command.CanExecute;

                return Observable.Return(true);
            })
            .Switch();
        NextButton.Command = ReactiveCommand.CreateFromTask(NextPage, canGoForward);
        NextButton[!ContentProperty] = tabAndPagesChanged
            .Select(x => x.Pages?[x.Index]
                    .GetObservable(PageData.NextButtonTextProperty)
                    .Select(str => str ?? DefaultNextButtonName)
             ?? Observable.Return(DefaultNextButtonName))
            .Switch()
            .ToBinding();

        var canGoBack = tabIndexChanged.Select(index => index > 0);
        BackButton.Command = ReactiveCommand.Create(() => PageControl.TabIndex--, canGoBack);
        BackButton[!IsVisibleProperty] = canGoBack.ToBinding();

        tabAndPagesChanged
            .Subscribe(x => {
                if (x.Pages is null || x.Index < 0 || x.Index >= x.Pages.Count) return;

                var dataTemplate = x.Pages[x.Index].DataTemplate;
                if (dataTemplate.Match(DataContext)) {
                    PageControl.Content = dataTemplate.Build(DataContext);
                }
            })
            .DisposeWith(ActivatedDisposable);
    }

    private async Task NextPage() {
        if (Pages is null) return;

        const string loadSpinnerClass = "LoadSpinner";
        NextButton.Classes.Add(loadSpinnerClass);

        // If the page has a command, execute it
        if ((HasFinalButton
                ? PageControl.TabIndex
                : PageControl.TabIndex + 1) < Pages.Count
         && Pages[PageControl.TabIndex] is { OnNext: {} command }) {
            await command.Execute();
        }

        NextButton.Classes.Remove(loadSpinnerClass);

        // If there are more pages, go to the next one
        if (PageControl.TabIndex + 1 < Pages.Count) {
            PageControl.TabIndex++;
        }
    }
}
