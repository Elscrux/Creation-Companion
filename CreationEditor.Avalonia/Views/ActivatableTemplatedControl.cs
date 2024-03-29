﻿using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Noggog;
using ReactiveUI;
namespace CreationEditor.Avalonia.Views;

public class ActivatableTemplatedControl : TemplatedControl, IActivatableView, IDisposable {
    protected readonly IDisposableBucket ActivatedDisposable = new DisposableBucket();
    protected readonly IDisposableBucket TemplateDisposable = new DisposableBucket();
    private readonly IDisposable _activationDisposable;

    protected ActivatableTemplatedControl() {
        _activationDisposable = this.WhenActivated(disposables => {
            Disposable
                .Create(() => ActivatedDisposable.Clear())
                .DisposeWith(disposables);

            WhenActivated();
        });
    }

    protected virtual void WhenActivated() {}

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        TemplateDisposable.Clear();
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposing) return;

        ActivatedDisposable.Dispose();
        TemplateDisposable.Dispose();
        _activationDisposable.Dispose();
    }
}
