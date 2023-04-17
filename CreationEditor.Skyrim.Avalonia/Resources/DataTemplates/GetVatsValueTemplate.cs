using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using CreationEditor.Skyrim.Avalonia.Models.Record.Editor.Subrecord;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
namespace CreationEditor.Skyrim.Avalonia.Resources.DataTemplates;

public class GetVatsValueTemplate : CustomConditionDataTemplate<AGetVATSValueConditionData> {
    private enum VatsFunction {
        Weapon,
        WeaponOrList,
        Target,
        TargetOrList,
        TargetDistance,
        TargetPart,
        Action,
        IsSuccess,
        IsCritical,
        CriticalEffect,
        CriticalEffectOrList,
        IsFatal,
        ExplodePart,
        DismemberPart,
        CripplePart,
        WeaponType,
        IsStranger,
        IsParalyzingPalm,
        ProjectileType,
        DeliveryType,
        CastingType,
    }

    protected override void Apply(IObservable<IMajorRecordGetter?> context, IObservable<IQuestGetter?> questContext, EditableCondition condition, AGetVATSValueConditionData data, IList<Control> parameterControls) {
        parameterControls.Clear();
        var comboBox = new ComboBox {
            DataContext = condition,
            ItemsSource = Enum.GetValues<VatsFunction>().OrderBy(x => x.ToString()).ToList(),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            [ToolTip.TipProperty] = "Function",
        };

        comboBox.AddHandler(SelectingItemsControl.SelectionChangedEvent, (_, args) => {
            var vatsFunction = args.AddedItems.OfType<VatsFunction>().First();
            ConditionData newData = vatsFunction switch {
                VatsFunction.Action => new GetVATSValueActionConditionData(),
                VatsFunction.CastingType => new GetVATSValueCastingTypeConditionData(),
                VatsFunction.CripplePart => new GetVATSValueCripplePartConditionData(),
                VatsFunction.CriticalEffect => new GetVATSValueCriticalEffectConditionData(),
                VatsFunction.CriticalEffectOrList => new GetVATSValueCriticalEffectOrListConditionData(),
                VatsFunction.DeliveryType => new GetVATSValueDeliveryTypeConditionData(),
                VatsFunction.DismemberPart => new GetVATSValueDismemberPartConditionData(),
                VatsFunction.ExplodePart => new GetVATSValueExplodePartConditionData(),
                VatsFunction.IsCritical => new GetVATSValueIsCriticalConditionData(),
                VatsFunction.IsFatal => new GetVATSValueIsFatalConditionData(),
                VatsFunction.IsParalyzingPalm => new GetVATSValueIsParalyzingPalmConditionData(),
                VatsFunction.IsStranger => new GetVATSValueIsStrangerConditionData(),
                VatsFunction.IsSuccess => new GetVATSValueIsSuccessConditionData(),
                VatsFunction.ProjectileType => new GetVATSValueProjectileTypeConditionData(),
                VatsFunction.Target => new GetVATSValueTargetConditionData(),
                VatsFunction.TargetOrList => new GetVATSValueTargetOrListConditionData(),
                VatsFunction.TargetDistance => new GetVATSValueTargetDistanceConditionData(),
                VatsFunction.TargetPart => new GetVATSValueTargetPartConditionData(),
                VatsFunction.WeaponOrList => new GetVATSValueWeaponOrListConditionData(),
                VatsFunction.WeaponType => new GetVATSValueWeaponTypeConditionData(),
                VatsFunction.Weapon => new GetVATSValueWeaponConditionData(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (condition.Data.GetType() == newData.GetType()) return;

            condition.Data = newData;
        });
        parameterControls.Add(comboBox);

        comboBox.SelectedItem = data switch {
            GetVATSValueActionConditionData => VatsFunction.Action,
            GetVATSValueCastingTypeConditionData => VatsFunction.CastingType,
            GetVATSValueCripplePartConditionData => VatsFunction.CripplePart,
            GetVATSValueCriticalEffectConditionData => VatsFunction.CriticalEffect,
            GetVATSValueCriticalEffectOrListConditionData => VatsFunction.CriticalEffectOrList,
            GetVATSValueDeliveryTypeConditionData => VatsFunction.DeliveryType,
            GetVATSValueDismemberPartConditionData => VatsFunction.DismemberPart,
            GetVATSValueExplodePartConditionData => VatsFunction.ExplodePart,
            GetVATSValueIsCriticalConditionData => VatsFunction.IsCritical,
            GetVATSValueIsFatalConditionData => VatsFunction.IsFatal,
            GetVATSValueIsParalyzingPalmConditionData => VatsFunction.IsParalyzingPalm,
            GetVATSValueIsStrangerConditionData => VatsFunction.IsStranger,
            GetVATSValueIsSuccessConditionData => VatsFunction.IsSuccess,
            GetVATSValueProjectileTypeConditionData => VatsFunction.ProjectileType,
            GetVATSValueTargetConditionData => VatsFunction.Target,
            GetVATSValueTargetOrListConditionData => VatsFunction.TargetOrList,
            GetVATSValueTargetDistanceConditionData => VatsFunction.TargetDistance,
            GetVATSValueTargetPartConditionData => VatsFunction.TargetPart,
            GetVATSValueWeaponOrListConditionData => VatsFunction.WeaponOrList,
            GetVATSValueWeaponTypeConditionData => VatsFunction.WeaponType,
            GetVATSValueWeaponConditionData => VatsFunction.Weapon,
            _ => throw new ArgumentOutOfRangeException()
        };

        var type = condition.Data.GetType();
        const string valueName = "Value";
        var value = type.GetProperty(valueName);
        if (value == null) return;

        Control? control = null;
        if (value.PropertyType.InheritsFrom(typeof(IFormLinkGetter))) {
            control = GetFormKeyPicker(condition.Data, valueName, nameof(IFormLinkGetter.FormKey), value.PropertyType.GenericTypeArguments);
        } else if (value.PropertyType.InheritsFrom(typeof(Enum))) {
            control = new ComboBox {
                DataContext = condition.Data,
                ItemsSource = Enum.GetValues(value.PropertyType).Cast<Enum>().OrderBy(x => x.ToString()).ToList(),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                [!SelectingItemsControl.SelectedItemProperty] = new Binding(valueName),
                [ToolTip.TipProperty] = valueName,
            };
        }

        // Needs margin because the control is hidden sometimes for some reason
        if (control != null) parameterControls.Add(new Grid { Margin = new Thickness(1), Children = { control } });
    }
}
