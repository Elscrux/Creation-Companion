using CreationEditor.Avalonia.Models.Mod.Editor;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord;

public enum BookTeaches {
    Nothing,
    Skill,
    Spell,
}

[IReactiveObject]
public sealed partial class EditableBook : Book, IEditableRecord<Book> {
    public static BookTeaches[] BookTeachesValues { get; } = Enum.GetValues<BookTeaches>();

    public override string? EditorID { get; set; }

    public string? NameStr {
        get => Name?.String;
        set {
            Name = value;
            this.RaisePropertyChanged();
        }
    }

    public bool CantBeTaken {
        get => (Flags & Flag.CantBeTaken) != 0;
        set {
            Flags = Flags.SetFlag(Flag.CantBeTaken, value);
            this.RaisePropertyChanged();
        }
    }

    public BookTeaches TeachesOption {
        get;
        set {
            field = value;
            this.RaisePropertyChanged();
        }
    }
    public Skill? Skill { get; set; }
    public BookSkill BookSkill { get; } = new();
    public BookSpell BookSpell { get; } = new();

    public EditableBook(IBookGetter parent) {
        FormKey = parent.FormKey;
        this.DeepCopyIn(parent);

        switch (parent.Teaches) {
            case BookSkill bookSkill:
                TeachesOption = BookTeaches.Skill;
                Skill = bookSkill.Skill;
                break;
            case BookSpell bookSpell:
                TeachesOption = BookTeaches.Spell;
                BookSpell.Spell.FormKey = bookSpell.Spell.FormKey;
                break;
            case null:
            case BookTeachesNothing:
                TeachesOption = BookTeaches.Nothing;
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    public void CopyTo(Book book) {
        book.DeepCopyIn(this);
        book.Description = Description is null || Description.All(x => string.IsNullOrEmpty(x.Value)) ? null : Description;
        book.Teaches = TeachesOption switch {
            BookTeaches.Nothing => new BookTeachesNothing {
                RawContent = uint.MaxValue,
            },
            BookTeaches.Skill => BookSkill,
            BookTeaches.Spell => BookSpell,
            _ => throw new InvalidOperationException(),
        };
    }
}
