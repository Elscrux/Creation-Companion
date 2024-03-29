﻿using System.Reactive;
using CreationEditor.Services.Settings;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.ViewModels.Setting;

public interface ISettingsVM : IDisposableDropoff {
    public IEnumerable<ISetting> RootSettings { get; }
    public ISetting? SelectedSetting { get; set; }

    public ReactiveCommand<Unit, Unit> Save { get; }
}
