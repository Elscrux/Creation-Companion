using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Book;
using ReactiveUI;
using TextMateSharp.Grammars;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Book;

public partial class BookEditor : ReactiveUserControl<BookEditorVM> {
    private readonly Subject<string> _textChanged = new();

    public BookEditor() {
        InitializeComponent();

        SetupEditor();

        this.WhenActivated(disposable => {
            this.WhenAnyValue(view => view.DataContext)
                .WhereNotNull()
                .OfType<BookEditorVM>()
                .Subscribe(vm => {
                    TextEditor.Document = new TextDocument(vm.BookText ?? string.Empty);
                    vm.BookText = TextEditor.Document.Text;

                    vm.WhenAnyValue(x => x.Language)
                        .Subscribe(_ => {
                            TextEditor.Document.Text = vm.EditableRecord.BookText.TryLookup(vm.Language, out var bookText)
                                ? bookText
                                : vm.EditableRecord.BookText;
                        })
                        .DisposeWith(disposable);

                    _textChanged
                        .ThrottleShort()
                        .Subscribe(text => vm.BookText = text)
                        .DisposeWith(disposable);
                })
                .DisposeWith(disposable);
        });
    }

    public BookEditor(BookEditorVM vm) : this() {
        DataContext = vm;
    }

    private void SetupEditor() {
        var registryOptions = new RegistryOptions(ThemeName.DarkPlus);
        var textMateInstallation = TextEditor.InstallTextMate(registryOptions);
        var scopeByLanguageId = registryOptions.GetScopeByLanguageId("markdown");
        textMateInstallation.SetGrammar(scopeByLanguageId);

        TextEditor.TextChanged += (_, _) => _textChanged.OnNext(TextEditor.Document.Text);
    }

    private void Switch_OnClick(object? sender, RoutedEventArgs e) {
        var previewRow = Preview.GetValue(Grid.RowProperty);
        var editorRow = TextEditor.GetValue(Grid.RowProperty);

        PreviewGrid.RowDefinitions[editorRow].MinHeight = 250;
        PreviewGrid.RowDefinitions[previewRow].MinHeight = 50;

        Preview[Grid.RowProperty] = editorRow;
        TextEditor[Grid.RowProperty] = previewRow;
    }
}
