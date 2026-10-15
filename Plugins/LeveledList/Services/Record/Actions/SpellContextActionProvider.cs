using Avalonia.Controls;
using CreationEditor;
using CreationEditor.Avalonia.Services.Actions;
using CreationEditor.Avalonia.Services.Record.Prefix;
using CreationEditor.Services.Environment;
using CreationEditor.Services.Mutagen.Record;
using CreationEditor.Skyrim;
using FluentAvalonia.UI.Controls;
using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Noggog;
using ReactiveUI.SourceGenerators;
using Serilog;
namespace LeveledList.Services.Record.Actions;

public sealed partial class SpellContextActionProvider : IContextActionsProvider {
    private static readonly ContextActionParent GenerateParent = new("Generate", FASymbol.Add);

    private readonly ILogger _logger;
    private readonly IRecordController _recordController;
    private readonly ILinkCacheProvider _linkCacheProvider;
    private readonly IRecordPrefixService _recordPrefixService;
    private readonly IList<ContextAction> _actions;

    public SpellContextActionProvider(
        ILogger logger,
        IRecordController recordController,
        ILinkCacheProvider linkCacheProvider,
        IRecordPrefixService recordPrefixService) {
        _logger = logger;
        _recordController = recordController;
        _linkCacheProvider = linkCacheProvider;
        _recordPrefixService = recordPrefixService;

        _actions = [
            new ContextAction(
                c => c.SelectedAssets.Count == 0
                 && c.SelectedRecords
                        .Select(r => r.ReferencedRecord.Record)
                        .OfType<ISpellGetter>()
                        .Any(spell => spell.CastType == CastType.FireAndForget
                         && spell.EquipmentType.FormKey != Skyrim.EquipType.LeftHand.FormKey
                         && spell.EquipmentType.FormKey != Skyrim.EquipType.RightHand.FormKey),
                10,
                ContextActionGroup.Modification,
                GenerateScrollCommand,
                context => new MenuItem {
                    Header = "Scroll",
                    Icon = new FASymbolIcon { Symbol = FASymbol.Document },
                    Command = GenerateScrollCommand,
                    CommandParameter = context,
                },
                Parent: GenerateParent),
            new ContextAction(
                c => c.SelectedAssets.Count == 0
                 && c.SelectedRecords
                        .Select(r => r.ReferencedRecord.Record)
                        .OfType<ISpellGetter>()
                        .Any(spell => spell.EquipmentType.FormKey != Skyrim.EquipType.LeftHand.FormKey
                         && spell.EquipmentType.FormKey != Skyrim.EquipType.RightHand.FormKey),
                9,
                ContextActionGroup.Modification,
                GenerateSpellTomeCommand,
                context => new MenuItem {
                    Header = "Spell Tome",
                    Icon = new FASymbolIcon { Symbol = FASymbol.Library },
                    Command = GenerateSpellTomeCommand,
                    CommandParameter = context,
                },
                Parent: GenerateParent),
        ];
    }

    [ReactiveCommand]
    private void GenerateScroll(SelectedListContext context) {
        foreach (var recordContext in context.SelectedRecords) {
            if (recordContext.ReferencedRecord.Record is not ISpellGetter spell) continue;
            if (spell.CastType != CastType.FireAndForget) continue;
            if (spell.EquipmentType.FormKey == Skyrim.EquipType.LeftHand.FormKey || spell.EquipmentType.FormKey == Skyrim.EquipType.RightHand.FormKey) continue;

            // Try two options for the scroll editor ID: one with "Spell" trimmed from the start, and one with "Spell" trimmed from the end.
            // This is to account for different naming conventions that might be used in the modding community.
            var spellEditorIdWithoutPrefix = _recordPrefixService.RemovePrefix(spell.EditorID ?? string.Empty);
            var editorIdOption1 = spellEditorIdWithoutPrefix?.TrimStart("Spell", StringComparison.OrdinalIgnoreCase);
            var scrollEditorIDOption1 = _recordPrefixService.ApplyPrefix("Scroll" + editorIdOption1);
            var editorIdOption2 = spellEditorIdWithoutPrefix?.TrimStringFromEnd("Spell");
            var scrollEditorIDOption2 = _recordPrefixService.ApplyPrefix("Scroll" + editorIdOption2);
            string editorId;

            Scroll scroll;
            if (_linkCacheProvider.LinkCache.TryResolve<IScrollGetter>(scrollEditorIDOption1, out var existingScroll1)) {
                editorId = scrollEditorIDOption1;
                scroll = _recordController.GetOrAddOverride<Scroll, IScrollGetter>(existingScroll1);
            } else if (_linkCacheProvider.LinkCache.TryResolve<IScrollGetter>(scrollEditorIDOption2, out var existingScroll2)) {
                editorId = scrollEditorIDOption2;
                scroll = _recordController.GetOrAddOverride<Scroll, IScrollGetter>(existingScroll2);
            } else {
                editorId = scrollEditorIDOption1;
                scroll = _recordController.GetOrCreateRecord<Scroll, IScrollGetter>(scrollEditorIDOption1);
            }

            scroll.EditorID = editorId;
            var nameString = spell.Name?.String;
            if (nameString is null) {
                _logger.Here().Warning("Spell {SpellEditorID} has no name, generated scroll uses EditorID as name", spell.EditorID);
            }

            scroll.Name = "Scroll of " + (nameString ?? spell.EditorID);
            scroll.Weight = 0.5f;
            scroll.Value = GetValue(spell);
            scroll.BaseCost = spell.BaseCost;
            scroll.Flags = spell.Flags;
            scroll.Type = spell.Type;
            scroll.ChargeTime = spell.ChargeTime;
            scroll.CastType = spell.CastType;
            scroll.TargetType = spell.TargetType;
            scroll.CastDuration = spell.CastDuration;
            scroll.Range = spell.Range;
            scroll.EquipmentType = spell.EquipmentType.AsNullable();
            scroll.Keywords = [Skyrim.Keyword.VendorItemScroll];
            scroll.MenuDisplayObject = Skyrim.Static.HighPolyBookTest01.AsNullable();
            if (scroll.Model is null) {
                var random = new Random().Next(1, 7);
                scroll.Model = new Mutagen.Bethesda.Skyrim.Model {
                    File = @$"Clutter\Common\Scroll0{random}.nif",
                };
                scroll.ObjectBounds = random switch {
                    1 => new ObjectBounds { First = new P3Int16(-16, -4, -4), Second = new P3Int16(16, 4, 4) },
                    2 or 3 => new ObjectBounds { First = new P3Int16(-15, -4, -3), Second = new P3Int16(15, 4, 3) },
                    4 => new ObjectBounds { First = new P3Int16(-9, -3, -2), Second = new P3Int16(12, 3, 2) },
                    5 => new ObjectBounds { First = new P3Int16(-15, -4, -4), Second = new P3Int16(17, 4, 4) },
                    6 => new ObjectBounds { First = new P3Int16(-13, -6, -3), Second = new P3Int16(13, 6, 3) },
                    _ => throw new InvalidOperationException($"Invalid random number for scroll model: {random}"),
                };
            }
            scroll.Effects.ReplaceWith(spell.Effects.Select(CopyEffect));
        }
    }

    [ReactiveCommand]
    private void GenerateSpellTome(SelectedListContext context) {
        foreach (var recordContext in context.SelectedRecords) {
            if (recordContext.ReferencedRecord.Record is not ISpellGetter spell) continue;
            if (spell.EquipmentType.FormKey == Skyrim.EquipType.LeftHand.FormKey || spell.EquipmentType.FormKey == Skyrim.EquipType.RightHand.FormKey) continue;

            var spellEditorIdWithoutPrefix = _recordPrefixService.RemovePrefix(spell.EditorID ?? string.Empty);
            var editorId = spellEditorIdWithoutPrefix?.TrimStart("Spell", StringComparison.OrdinalIgnoreCase);
            var bookEditorId = _recordPrefixService.ApplyPrefix("SpellTome" + (editorId ?? string.Empty));

            var book = _recordController.GetOrCreateRecord<Book, IBookGetter>(bookEditorId);
            book.EditorID = bookEditorId;
            book.Name = "Spell Tome: " + (spell.Name?.String ?? spell.EditorID);
            book.Weight = 1f;
            book.Value = book.Value is 0 ? GetValue(spell) : book.Value;
            book.Keywords = [Skyrim.Keyword.VendorItemBook, Skyrim.Keyword.VendorItemSpellTome];

            var schoolOfMagic = spell.GetSchoolOfMagic(_linkCacheProvider.LinkCache);
            if (book.Model is null) {
                if (schoolOfMagic is null) {
                    schoolOfMagic = ActorValue.Destruction;
                    _logger.Here().Warning("Spell {SpellEditorID} couldn't detect a school of magic, the generated spell tome uses destruction model", spell.EditorID);
                }

                book.Model = new Mutagen.Bethesda.Skyrim.Model {
                    File = @$"Clutter\Books\SpellTome{schoolOfMagic}LowPoly.nif",
                };
                book.ObjectBounds = new ObjectBounds {
                    First = new P3Int16(-9, -12, -2),
                    Second = new P3Int16(8, 12, 2),
                };
            }

            if (book.InventoryArt.IsNull) {
                if (schoolOfMagic == ActorValue.None) {
                    schoolOfMagic = ActorValue.Destruction;
                    _logger.Here().Warning("Spell {SpellEditorID} couldn't detect a school of magic, the generated spell tome uses destruction inventory art",
                        spell.EditorID);
                }

                book.InventoryArt = schoolOfMagic switch {
                    ActorValue.Alteration => Skyrim.Static.HighPolyAlterationBook.AsNullable(),
                    ActorValue.Conjuration => Skyrim.Static.HighPolyConjurationBook.AsNullable(),
                    ActorValue.Destruction => Skyrim.Static.HighPolyDestructionBook.AsNullable(),
                    ActorValue.Illusion => Skyrim.Static.HighPolyIllusionBook.AsNullable(),
                    ActorValue.Restoration => Skyrim.Static.HighPolyRestorationBook.AsNullable(),
                    _ => throw new InvalidOperationException($"{spell.EditorID} has invalid school of magic: {schoolOfMagic}"),
                };
            }

            book.BookText = book.BookText.String.IsNullOrEmpty()
                ? new TranslatedString(Language.English,
                    $"""
                     <font face='$HandwrittenFont'><font size='40'><p align='center'>
                     {spell.Name?.String ?? spell.EditorID}
                     """)
                : book.BookText;
            book.Teaches = new BookSpell { Spell = spell.ToLink() };
        }
    }

    private static Effect CopyEffect(IEffectGetter effect) => new() {
        BaseEffect = new FormLinkNullable<IMagicEffectGetter>(effect.BaseEffect.FormKey),
        Data = new EffectData {
            Magnitude = effect.Data?.Magnitude ?? 0,
            Area = effect.Data?.Area ?? 0,
            Duration = effect.Data?.Duration ?? 0,
        },
    };

    private uint GetValue(ISpellGetter spell) {
        // Best effort value estimation based on the spell's magic level,
        // mirroring vanilla spell tome values
        var level = spell.GetMagicLevel(_linkCacheProvider.LinkCache);
        return level switch {
            < 25 => 25,
            < 50 => 50,
            < 75 => 100,
            < 100 => 250,
            _ => 500,
        };
    }

    public IEnumerable<ContextAction> GetActions() => _actions;
}
