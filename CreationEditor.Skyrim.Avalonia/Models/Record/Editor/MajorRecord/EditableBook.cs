using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Models.Record.Editor.MajorRecord;

public enum BookTeaches {
    Nothing,
    Skill,
    Spell,
}

public sealed class EditableBook : Book, INotifyPropertyChanged {
    public static BookTeaches[] BookTeachesValues { get; } = Enum.GetValues<BookTeaches>();

    public override string? EditorID { get; set; }

    public string? NameStr {
        get => Name?.String;
        set {
            Name = value;
            OnPropertyChanged();
        }
    }

    public bool CantBeTaken {
        get => (Flags & Flag.CantBeTaken) != 0;
        set {
            Flags = Flags.SetFlag(Flag.CantBeTaken, value);
            OnPropertyChanged();
        }
    }

    private BookTeaches _teachesOption;
    public BookTeaches TeachesOption {
        get => _teachesOption;
        set {
            _teachesOption = value;
            OnPropertyChanged();
        }
    }
    public Skill? Skill { get; set; }
    public BookSkill BookSkill { get; } = new();
    public BookSpell BookSpell { get; } = new();

    public EditableBook(IBook parent) {
        EditorID = parent.EditorID;
        Name = parent.Name;
        FormKey = parent.FormKey;
        Flags = parent.Flags;
        Description = parent.Description;
        Value = parent.Value;
        Weight = parent.Weight;
        Keywords = parent.Keywords;
        BookText = parent.BookText;
        Destructible = parent.Destructible;
        InventoryArt = parent.InventoryArt;
        VirtualMachineAdapter = parent.VirtualMachineAdapter;
        ObjectBounds = parent.ObjectBounds;
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
        book.EditorID = EditorID;
        book.Name = Name;
        book.Flags = Flags;
        book.Description = Description is null || Description.All(x => string.IsNullOrEmpty(x.Value)) ? null : Description;
        book.Value = Value;
        book.Weight = Weight;
        book.Keywords = Keywords;
        book.BookText = BookText;
        book.Destructible = Destructible;
        book.InventoryArt = InventoryArt;
        book.VirtualMachineAdapter = VirtualMachineAdapter;
        book.ObjectBounds = ObjectBounds;
        book.Teaches = TeachesOption switch {
            BookTeaches.Nothing => new BookTeachesNothing {
                RawContent = uint.MaxValue,
            },
            BookTeaches.Skill => BookSkill,
            BookTeaches.Spell => BookSpell,
            _ => throw new InvalidOperationException(),
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
