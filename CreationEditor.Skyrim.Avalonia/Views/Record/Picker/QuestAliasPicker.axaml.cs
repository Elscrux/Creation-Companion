using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using CreationEditor.Avalonia.Views;
using DynamicData;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Picker;

public partial class QuestAliasPicker : ActivatableUserControl {
    public static readonly StyledProperty<IQuestGetter?> QuestProperty
        = AvaloniaProperty.Register<QuestAliasPicker, IQuestGetter?>(nameof(Quest));

    public static readonly StyledProperty<uint> AliasIndexProperty
        = AvaloniaProperty.Register<QuestAliasPicker, uint>(nameof(AliasIndex));

    public static readonly StyledProperty<ReadOnlyObservableCollection<IQuestAliasGetter>?> AliasesProperty
        = AvaloniaProperty.Register<QuestAliasPicker, ReadOnlyObservableCollection<IQuestAliasGetter>?>(nameof(Aliases));

    public static readonly StyledProperty<IQuestAliasGetter?> SelectedAliasProperty
        = AvaloniaProperty.Register<QuestAliasPicker, IQuestAliasGetter?>(nameof(SelectedAlias));

    public static readonly StyledProperty<IEnumerable<QuestAlias.TypeEnum>?> ScopedAliasTypesProperty
        = AvaloniaProperty.Register<QuestAliasPicker, IEnumerable<QuestAlias.TypeEnum>?>(nameof(ScopedAliasTypes));

    public IQuestGetter? Quest {
        get => GetValue(QuestProperty);
        set => SetValue(QuestProperty, value);
    }

    public uint AliasIndex {
        get => GetValue(AliasIndexProperty);
        set => SetValue(AliasIndexProperty, value);
    }

    public ReadOnlyObservableCollection<IQuestAliasGetter>? Aliases {
        get => GetValue(AliasesProperty);
        set => SetValue(AliasesProperty, value);
    }

    public IQuestAliasGetter? SelectedAlias {
        get => GetValue(SelectedAliasProperty);
        set => SetValue(SelectedAliasProperty, value);
    }

    public IEnumerable<QuestAlias.TypeEnum>? ScopedAliasTypes {
        get => GetValue(ScopedAliasTypesProperty);
        set => SetValue(ScopedAliasTypesProperty, value);
    }

    public QuestAliasPicker() {
        InitializeComponent();
    }

    protected override void WhenActivated() {
        // Update the selected alias when index or quest changes
        this.WhenAnyValue(
                x => x.AliasIndex,
                x => x.Quest,
                (index, quest) => (Index: index, Quest: quest))
            .Subscribe(x => {
                var (index, quest) = x;
                SelectedAlias = quest?.Aliases switch {
                    [] => null,
                    _ => quest?.Aliases.FirstOrDefault(alias => alias.ID == index) ?? quest?.Aliases[0]
                };
            })
            .DisposeWith(ActivatedDisposable);

        // Populate aliases when quest changes
        Aliases = this.WhenAnyValue(
                x => x.Quest,
                x => x.ScopedAliasTypes,
                (quest, scopedAliasTypes) => (Quest: quest, ScopedAliasTypes: scopedAliasTypes))
            .Select(x => {
                if (x.Quest is null) return null;
                if (x.ScopedAliasTypes is null) return x.Quest.Aliases.AsObservableChangeSet();

                return x.Quest.Aliases
                    .Where(alias => x.ScopedAliasTypes.Contains(alias.Type))
                    .AsObservableChangeSet();
            })
            .NotNull()
            .Switch()
            .ToObservableCollection(ActivatedDisposable);

        // Update the index when the selected alias changes
        this.WhenAnyValue(x => x.SelectedAlias)
            .NotNull()
            .Subscribe(alias => AliasIndex = alias.ID)
            .DisposeWith(ActivatedDisposable);
    }
}
