using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Avalonia.Views;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Book;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Book.Preview;

public partial class BookPreview : ActivatableUserControl {
    public const double LineSpacing = -2.4765;

    public const double PageHeight = 480;
    public const double DoublePageHeight = PageHeight * 2;
    public const double PageWidth = 295.9;
    public const double PageMarginsSize = 10;
    public static readonly Thickness PageMargins = new(PageMarginsSize);
    public const double PageLeftOffsetMarginsSize = PageWidth + PageMarginsSize;
    public static readonly Thickness LeftPageMargins = new(-PageLeftOffsetMarginsSize, 0, 0, 0);
    public static readonly Thickness RightPageMargins = new(PageLeftOffsetMarginsSize, 0, 0, 0);

    public const double BookHeight = 473 + BookMarginsVertical * 2 + PageMarginsSize * 2;
    public const double BookWidth = PageWidth * 2 + BookMarginsHorizontal * 2 + PageMarginsSize * 4;
    public const double BookMarginsHorizontal = 20;
    public const double BookMarginsVertical = 10;
    public static readonly Thickness BookMargins = new(BookMarginsHorizontal, BookMarginsVertical);

    private double _totalTextHeight;
    private int _currentLeftPage;
    private HtmlConverter? _htmlConverter;

    public BookPreview() {
        InitializeComponent();
    }

    public BookPreview(BookEditorVM vm) : this() {
        DataContext = vm;
    }

    private void ScrollDefault() {
        LeftPage.Margin = new Thickness(LeftPage.Margin.Left, 0, LeftPage.Margin.Right, LeftPage.Margin.Bottom);
        RightPage.Margin = new Thickness(RightPage.Margin.Left, -PageHeight, RightPage.Margin.Right, RightPage.Margin.Bottom);
    }

    protected override void WhenActivated() {
        base.WhenActivated();

        this.WhenAnyValue(view => view.DataContext)
            .NotNull()
            .OfType<BookEditorVM>()
            .Subscribe(vm => {
                UpdatePreview(vm.BookText);
                ScrollDefault();
                _htmlConverter = vm.CreateHtmlConverter(new HtmlConverterOptions(PageWidth, LineSpacing));

                vm.WhenAnyValue(x => x.BookText)
                    .Subscribe(text => {
                        vm.BookText = text;
                        UpdatePreview(text);
                    })
                    .DisposeWith(ActivatedDisposable);
            })
            .DisposeWith(ActivatedDisposable);

        // Scale the book to fit
        BookBackground.RenderTransform = new ScaleTransform(1, 1);
        Grid.GetObservable(BoundsProperty)
            .Subscribe(bounds => {
                var scaleY = bounds.Height / BookBackground.Bounds.Height;
                var scaleX = bounds.Width / BookBackground.Bounds.Width;

                var scale = Math.Min(scaleX, scaleY) - 0.05;

                BookBackground.RenderTransform = new ScaleTransform(scale, scale);
            })
            .DisposeWith(ActivatedDisposable);
    }

    private void Back(object? sender, RoutedEventArgs e) {
        Scroll(LeftPage, RightPage, false);
    }

    private void Next(object? sender, RoutedEventArgs e) {
        Scroll(LeftPage, RightPage, true);
    }

    public void Scroll(Control leftPage, Control rightPage, bool up) {
        if (!CanScroll(up)) return;

        _currentLeftPage += up ? 2 : -2;
        leftPage.Margin = new Thickness(leftPage.Margin.Left, -_currentLeftPage * PageHeight, leftPage.Margin.Right, leftPage.Margin.Bottom);
        rightPage.Margin = new Thickness(
            rightPage.Margin.Left,
            -(_currentLeftPage + 1) * PageHeight,
            rightPage.Margin.Right,
            rightPage.Margin.Bottom);

        UpdateButtons();
    }

    private void UpdateButtons() {
        BackButton.IsEnabled = CanScroll(false);
        NextButton.IsEnabled = CanScroll(true);
    }

    private bool CanScroll(bool up) {
        if (up) {
            return -LeftPage.Margin.Top + DoublePageHeight < _totalTextHeight;
        }

        return _currentLeftPage != 0;
    }

    private void UpdatePreview(string? text) {
        if (text is null || _htmlConverter is null) return;

        var size = new Size(PageWidth, PageHeight * 100);

        var leftControls = _htmlConverter.GenerateControls(text).ToList();
        foreach (var control in leftControls) {
            control.Measure(size);
            var desiredSize = control.DesiredSize;
            var pageCount = (int) Math.Ceiling(desiredSize.Height / PageHeight);
            control.MinHeight = pageCount * PageHeight;
        }

        var rightControls = _htmlConverter.GenerateControls(text).ToList();
        foreach (var control in rightControls) {
            control.Measure(size);
            var desiredSize = control.DesiredSize;
            var pageCount = (int) Math.Ceiling(desiredSize.Height / PageHeight);
            control.MinHeight = pageCount * PageHeight;
        }

        _totalTextHeight = leftControls.Sum(c => c.MinHeight);

        Dispatcher.UIThread.Post(() => {
            LeftPage.Children.ReplaceWith(leftControls);
            RightPage.Children.ReplaceWith(rightControls);
            UpdateButtons();
        });
    }
}
