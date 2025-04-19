using System.Reactive.Linq;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using CreationEditor.Avalonia.Views;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Book;
using Mutagen.Bethesda.Strings;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Book.Preview;

public partial class NotePreview : ActivatableUserControl {
    public const double PageHeight = 475;
    public const double PageWidth = 395;

    public const double NoteMarginsSize = 20;
    public static readonly Thickness NoteMargins = new(NoteMarginsSize);
    public const double NoteWidth = PageWidth + NoteMarginsSize * 2;
    public const double NoteHeight = PageHeight + NoteMarginsSize * 2;
    public const double LineSpacing = -2.4765;

    private double _totalTextHeight;
    private int _currentLeftPage;
    private Func<Language, HtmlConverter>? _htmlConverterFactory;

    public NotePreview() {
        InitializeComponent();
    }

    private void ScrollDefault() {
        Page.Margin = new Thickness(Page.Margin.Left, 0, Page.Margin.Right, Page.Margin.Bottom);
    }

    protected override void WhenActivated() {
        base.WhenActivated();

        this.WhenAnyValue(view => view.DataContext)
            .NotNull()
            .OfType<BookEditorVM>()
            .Subscribe(vm => {
                UpdatePreview(vm.Language, vm.BookText);
                ScrollDefault();
                _htmlConverterFactory = language => vm.CreateHtmlConverter(new HtmlConverterOptions(language, PageWidth, LineSpacing));

                vm.WhenAnyValue(x => x.BookText)
                    .Subscribe(text => {
                        vm.BookText = text;
                        UpdatePreview(vm.Language, text);
                    })
                    .DisposeWith(ActivatedDisposable);
            })
            .DisposeWith(ActivatedDisposable);

        // Scale the book to fit
        NoteBackground.RenderTransform = new ScaleTransform(1, 1);
        Grid.GetObservable(BoundsProperty)
            .Subscribe(bounds => {
                var scaleY = bounds.Height / NoteBackground.Bounds.Height;
                var scaleX = bounds.Width / NoteBackground.Bounds.Width;

                var scale = Math.Min(scaleX, scaleY) - 0.05;

                NoteBackground.RenderTransform = new ScaleTransform(scale, scale);
            })
            .DisposeWith(ActivatedDisposable);
    }

    private void Back(object? sender, RoutedEventArgs e) => Scroll(false);
    private void Next(object? sender, RoutedEventArgs e) => Scroll(true);

    public void Scroll(bool up) {
        if (!CanScroll(up)) return;

        _currentLeftPage += up ? 1 : -1;
        Page.Margin = new Thickness(Page.Margin.Left, -_currentLeftPage * PageHeight, Page.Margin.Right, Page.Margin.Bottom);

        UpdateButtons();
    }

    private void UpdateButtons() {
        BackButton.IsEnabled = CanScroll(false);
        NextButton.IsEnabled = CanScroll(true);
    }

    private bool CanScroll(bool up) {
        if (up) {
            return -Page.Margin.Top + PageHeight < _totalTextHeight;
        }

        return _currentLeftPage != 0;
    }

    private void UpdatePreview(Language language, string? text) {
        if (text is null || _htmlConverterFactory is null) return;

        var maxSize = new Size(PageWidth, PageHeight * 100);

        var htmlConverter = _htmlConverterFactory(language);
        var controls = htmlConverter.GenerateControls(text).ToList();
        foreach (var control in controls) {
            control.Measure(maxSize);
            var desiredSize = control.DesiredSize;
            var pageCount = (int) Math.Ceiling(desiredSize.Height / PageHeight);
            control.MinHeight = pageCount * PageHeight;
        }

        _totalTextHeight = controls.Sum(c => c.MinHeight);

        Dispatcher.UIThread.Post(() => {
            Page.Children.ReplaceWith(controls);
            UpdateButtons();
        });
    }
}
