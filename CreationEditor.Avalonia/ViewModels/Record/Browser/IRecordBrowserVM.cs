﻿using System.Reactive;
using CreationEditor.Avalonia.Models.Record.Browser;
using CreationEditor.Avalonia.Views.Record;
using DynamicData.Binding;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public interface IRecordBrowserVM : IDisposableDropoff {
    public IObservableCollection<RecordTypeGroup> RecordTypeGroups { get; }

    [Reactive] public RecordList? RecordList { get; set; }

    public ReactiveCommand<RecordTypeGroup, Unit> SelectRecordTypeGroup { get; }
    public ReactiveCommand<RecordTypeListing, Unit> SelectRecordType { get; }
    public ReactiveCommand<RecordFilterListing, Unit> SelectRecordFilter { get; }

    public IRecordBrowserSettingsVM RecordBrowserSettingsVM { get; }
}
