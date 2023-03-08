using Avalonia.Controls;
namespace CreationEditor.Avalonia.Models.Record.List.ExtraColumns;

public sealed record ExtraColumn(DataGridColumn Column, byte Priority);
