﻿using System.Reactive;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.ViewModels.Record.List;
using CreationEditor.Avalonia.Views.Record;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public interface IRecordBrowserVM : IDisposableDropoff {
    public IObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }

    public ReactiveCommand<RecordTypeGroup, Unit> SelectRecordTypeGroup { get; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }
    public ReactiveCommand<RecordFilterListing, Unit> SelectRecordFilter { get; }

    public IRecordListVM? RecordListVM { get; set; }
    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }
}
