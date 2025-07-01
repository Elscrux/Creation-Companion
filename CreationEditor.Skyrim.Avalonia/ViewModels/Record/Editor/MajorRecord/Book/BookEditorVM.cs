using System.Reactive.Linq;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord;
using CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Book.Preview;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Book;

public sealed partial class BookEditorVM : ViewModel, IRecordEditorVM<Mutagen.Bethesda.Skyrim.Book, IBookGetter> {
    private readonly Func<HtmlConverterOptions, HtmlConverter> _htmlConverterFactory;

    IRecordEditorCore IRecordEditorVM.Core => Core;
    public IRecordEditorCore<EditableBook, Mutagen.Bethesda.Skyrim.Book, IBookGetter> Core { get; }

    [Reactive] public partial Language Language { get; set; }
    [Reactive] public partial string? BookText { get; set; }
    [Reactive] public partial string? Description { get; set; }
    [Reactive] public partial FormKey InventoryArt { get; set; }

    public IObservable<bool> IsNote { get; }

    public BookEditorVM(
        Mutagen.Bethesda.Skyrim.Book book,
        Func<Mutagen.Bethesda.Skyrim.Book, EditableRecordConverter<EditableBook, Mutagen.Bethesda.Skyrim.Book, IBookGetter>, IRecordEditorCore<EditableBook, Mutagen.Bethesda.Skyrim.Book, IBookGetter>> coreFactory,
        Func<HtmlConverterOptions, HtmlConverter> htmlConverterFactory) {
        _htmlConverterFactory = htmlConverterFactory;

        var converter = new EditableRecordConverter<EditableBook, Mutagen.Bethesda.Skyrim.Book, IBookGetter>(
            b => new EditableBook(b),
            b => b.DeepCopy());
        Core = coreFactory(book, converter).DisposeWith(this);

        IsNote = this.WhenAnyValue(x => x.InventoryArt)
            .Select(x => Core.LinkCacheProvider.LinkCache.TryResolveIdentifier<IStaticGetter>(x, out var editorId)
             && editorId is not null
             && editorId.Contains("note", StringComparison.OrdinalIgnoreCase));

        this.WhenAnyValue(
                x => x.Core.EditableRecord,
                x => x.Language,
                (record, language) => (Record: record, Language: language))
            .Subscribe(x => {
                InventoryArt = x.Record.InventoryArt.FormKey;
                Description = x.Record.Description?.GetLanguageStringOrDefault(x.Language);
                BookText = x.Record.BookText.GetLanguageStringOrDefault(x.Language);
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.BookText)
            .Subscribe(text => Core.EditableRecord.BookText.Set(Language, text))
            .DisposeWith(this);

        this.WhenAnyValue(x => x.Description)
            .NotNull()
            .Subscribe(text => {
                Core.EditableRecord.Description ??= new TranslatedString(Language);
                Core.EditableRecord.Description.Set(Language, text);
            })
            .DisposeWith(this);
    }

    public HtmlConverter CreateHtmlConverter(HtmlConverterOptions options) => _htmlConverterFactory(options);
}
