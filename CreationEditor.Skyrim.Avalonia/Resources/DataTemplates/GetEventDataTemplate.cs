using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using CreationEditor.Avalonia.Converter;
using CreationEditor.Avalonia.FormKeyPicker;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using CreationEditor.Skyrim.Definitions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Resources.DataTemplates;

public class GetEventDataTemplate : CustomConditionDataTemplate<GetEventDataConditionData> {
    public override void Apply(IObservable<IMajorRecordGetter?> context, IObservable<IQuestGetter?> questContext, EditableCondition condition, GetEventDataConditionData data, IList<Control> parameterControls) {
        if (parameterControls is not [ComboBox functionBox, ComboBox, FormKeyPicker formKeyPicker]) return;

        var functionChanged = functionBox.GetObservable(SelectingItemsControl.SelectedItemProperty);
        functionChanged.Subscribe((selected => {
            if (selected is not GetEventDataConditionData.EventFunction eventFunction) return;

            // Clear the form key picker
            formKeyPicker.FormKey = FormKey.Null;

            // Update the the scoped types depending on the selected function
            switch (eventFunction) {
                case GetEventDataConditionData.EventFunction.GetIsID:
                    formKeyPicker.ScopedTypes = typeof(IReferenceableObjectGetter).AsEnumerable();
                    break;
                case GetEventDataConditionData.EventFunction.IsInList:
                    formKeyPicker.IsVisible = true;
                    formKeyPicker.ScopedTypes = typeof(IFormListGetter).AsEnumerable();
                    break;
                case GetEventDataConditionData.EventFunction.HasKeyword:
                    formKeyPicker.IsVisible = true;
                    formKeyPicker.ScopedTypes = typeof(IKeywordGetter).AsEnumerable();
                    break;
                default:
                    formKeyPicker.IsVisible = false;
                    break;
            }
        }));

        questContext.Subscribe(quest => {
            if (quest?.Event == null) return;

            // Get the event definition
            var storyManagerEvent = SkyrimDefinitions.StoryManagerEvents.FirstOrDefault(e => e.Type == quest.Event.Value.TypeInt);
            if (storyManagerEvent is null || storyManagerEvent.NonReferenceEnums.Count == 0) return;

            // Initialize the member to the first valid value if it's not set
            var firstValue = storyManagerEvent.NonReferenceEnums.First();
            if (data.Member == GetEventDataConditionData.EventMember.None) {
                data.Member = (GetEventDataConditionData.EventMember) firstValue;
            }

            var enumToEventMember = new Func<Enum?, GetEventDataConditionData.EventMember>(
                e => (GetEventDataConditionData.EventMember) Enum.ToObject(
                    typeof(GetEventDataConditionData.EventMember),
                    e == null
                        ? -1
                        : Convert.ToUInt16(e)));

            // Update the member picker
            const string path = "Member";
            var enumType = firstValue.GetType();
            var box = new ComboBox {
                DataContext = data,
                ItemsSource = storyManagerEvent.NonReferenceEnums as IList,
                [!SelectingItemsControl.SelectedItemProperty] = new Binding(path) {
                    Converter = new ExtendedFuncValueConverter<GetEventDataConditionData.EventMember, Enum, object>(
                        (member, _) => (Enum) Enum.ToObject(enumType, Convert.ToUInt16(member)),
                        (e, _) => enumToEventMember(e)),
                    Mode = BindingMode.TwoWay
                },
                HorizontalAlignment = HorizontalAlignment.Stretch,
                [ToolTip.TipProperty] = path,
            };

            parameterControls.RemoveAt(1);
            parameterControls.Insert(1, box);

            // Update the form key picker's scope when the member or function changes
            box.GetObservable(SelectingItemsControl.SelectedItemProperty)
                .CombineLatest(functionChanged, (member, function) => (Member: member, Function: function))
                .Subscribe(x => {
                    if (x.Function is not GetEventDataConditionData.EventFunction eventFunction) return;
                    if (eventFunction is not GetEventDataConditionData.EventFunction.GetIsID) return;

                    switch (enumToEventMember(x.Member as Enum)) {
                        case GetEventDataConditionData.EventMember.CreatedObject:
                            formKeyPicker.IsVisible = true;
                            formKeyPicker.ScopedTypes = typeof(IConstructibleGetter).AsEnumerable();
                            break;
                        case GetEventDataConditionData.EventMember.OldLocation:
                        case GetEventDataConditionData.EventMember.NewLocation:
                            formKeyPicker.IsVisible = true;
                            formKeyPicker.ScopedTypes = typeof(ILocationGetter).AsEnumerable();
                            break;
                        case GetEventDataConditionData.EventMember.Keyword:
                            formKeyPicker.IsVisible = true;
                            formKeyPicker.ScopedTypes = typeof(IKeywordGetter).AsEnumerable();
                            break;
                        case GetEventDataConditionData.EventMember.Form:
                            formKeyPicker.IsVisible = true;
                            formKeyPicker.ScopedTypes = typeof(IReferenceableObjectGetter).AsEnumerable();
                            break;
                        default:
                            formKeyPicker.IsVisible = false;
                            break;
                    }
                });
        });
    }
}
