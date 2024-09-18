using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Controls;
using CreationEditor.Avalonia.Services.Record.Editor;
using CreationEditor.Avalonia.ViewModels;
using CreationEditor.Avalonia.ViewModels.Record.Editor;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord;
using CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Book;
using CreationEditor.Skyrim.Avalonia.Views.Record.Editor.MajorRecord.Book.Preview;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Skyrim.Avalonia.ViewModels.Record.Editor.MajorRecord.Book;

public sealed class BookEditorVM : ViewModel, IRecordEditorVM<Mutagen.Bethesda.Skyrim.Book, IBookGetter> {
    private readonly Func<HtmlConverterOptions, HtmlConverter> _htmlConverterFactory;
    IMajorRecordGetter IRecordEditorVM.Record => Record;
    public Mutagen.Bethesda.Skyrim.Book Record { get; set; } = null!;
    [Reactive] public EditableBook? EditableRecord { get; set; }

    public ReactiveCommand<Unit, Unit> Save { get; }

    public ILinkCacheProvider LinkCacheProvider { get; }

    [Reactive] public Language Language { get; set; }
    [Reactive] public string? BookText { get; set; }
    [Reactive] public string? Description { get; set; }
    [Reactive] public FormKey InventoryArt { get; set; }

    public IObservable<bool> IsNote { get; }

    public BookEditorVM(
        Func<HtmlConverterOptions, HtmlConverter> htmlConverterFactory,
        IRecordEditorController recordEditorController,
        IRecordController recordController,
        ILinkCacheProvider linkCacheProvider) {
        _htmlConverterFactory = htmlConverterFactory;
        LinkCacheProvider = linkCacheProvider;

        IsNote = this.WhenAnyValue(x => x.InventoryArt)
            .Select(x => LinkCacheProvider.LinkCache.TryResolveIdentifier<IStaticGetter>(x, out var editorId)
             && editorId is not null
             && editorId.Contains("note", StringComparison.OrdinalIgnoreCase));

        this.WhenAnyValue(x => x.EditableRecord, x => x.Language, (record, language) => (Record: record, Language: language))
            .Subscribe(x => {
                if (x.Record is null) return;

                InventoryArt = x.Record.InventoryArt.FormKey;
                Description = x.Record.Description?.GetLanguageStringOrDefault(x.Language);
                BookText = x.Record.BookText.GetLanguageStringOrDefault(x.Language);
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.BookText)
            .Subscribe(text => {
                if (EditableRecord is null) return;

                EditableRecord.BookText.Set(Language, text);
            })
            .DisposeWith(this);

        this.WhenAnyValue(x => x.Description)
            .NotNull()
            .Subscribe(text => {
                if (EditableRecord is null) return;

                EditableRecord.Description ??= new TranslatedString(Language);
                EditableRecord.Description.Set(Language, text);
            })
            .DisposeWith(this);

        Save = ReactiveCommand.Create(() => {
            if (EditableRecord is null) return;

            EditableRecord.InventoryArt.FormKey = InventoryArt;

            recordController.RegisterUpdate(Record, () => EditableRecord.CopyTo(Record));

            recordEditorController.CloseEditor(Record);
        });
    }

    public Control CreateControl(Mutagen.Bethesda.Skyrim.Book record) {
        Record = record;
        EditableRecord = new EditableBook(record);

        return new BookEditor(this);
    }

    public HtmlConverter CreateHtmlConverter(HtmlConverterOptions options) => _htmlConverterFactory(options);
}
