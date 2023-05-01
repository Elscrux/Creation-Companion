using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using CreationEditor.Avalonia.Views;
using CreationEditor.Skyrim.Definitions;
using DynamicData.Binding;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Skyrim.Avalonia.Views.Record.Picker;

public partial class EventDataPicker : ActivatableUserControl {
    public static readonly StyledProperty<IQuestGetter?> QuestProperty
        = AvaloniaProperty.Register<EventDataPicker, IQuestGetter?>(nameof(Quest));

    public static readonly StyledProperty<Enum?> EventMemberProperty
        = AvaloniaProperty.Register<EventDataPicker, Enum?>(nameof(EventMember));

    public static readonly StyledProperty<ReadOnlyObservableCollection<Enum>> EventMembersProperty
        = AvaloniaProperty.Register<EventDataPicker, ReadOnlyObservableCollection<Enum>>(nameof(EventMembers));

    public Enum? EventMember {
        get => GetValue(EventMemberProperty);
        set => SetValue(EventMemberProperty, value);
    }

    public IQuestGetter? Quest {
        get => GetValue(QuestProperty);
        set => SetValue(QuestProperty, value);
    }

    public ReadOnlyObservableCollection<Enum> EventMembers {
        get => GetValue(EventMembersProperty);
        set => SetValue(EventMembersProperty, value);
    }

    public EventDataPicker() {
        InitializeComponent();
    }

    protected override void WhenActivated() {
        var eventObservable = this.WhenAnyValue(x => x.Quest)
            .Select(quest => {
                if (quest?.Event == null) return null;

                var eventType = quest.Event.Value.TypeInt;
                return SkyrimDefinitions.StoryManagerEvents.FirstOrDefault(@event => @event.Type == eventType);
            });

        // Populate the list of event members with the members of the currently selected event in the context
        EventMembers = eventObservable
            .Select(storyManagerEvent => storyManagerEvent == null ? Array.Empty<Enum>() : storyManagerEvent.ReferenceEnums)
            .ToObservableCollection(ActivatedDisposable);

        // Force the event member to be the right type when the list of event members changes
        this.WhenAnyValue(x => x.EventMembers)
            .DistinctUntilChanged()
            .CombineLatest(EventMembers.ObserveCollectionChanges().Unit().StartWith(Unit.Default), (x, _) => x)
            .Subscribe(eventMembers => {
                // Force convert the event member to the right enum type
                if (EventMember == null) {
                    // Use the first value in the list in case the event member is null
                    EventMember = eventMembers.FirstOrDefault();
                } else if (eventMembers.Any()) {
                    EventMember = EventMember.ToEnum(eventMembers.First().GetType());
                } else {
                    EventMember = GetEventDataConditionData.EventMember.None;
                }
            })
            .DisposeWith(ActivatedDisposable);
    }
}
