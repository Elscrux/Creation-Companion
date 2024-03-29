﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.Models.Selectables;

public sealed class TypeItem(Type type, bool isSelected = true) : ReactiveObject, IReactiveSelectable {
    [Reactive] public bool IsSelected { get; set; } = isSelected;
    public Type Type { get; } = type;
}
