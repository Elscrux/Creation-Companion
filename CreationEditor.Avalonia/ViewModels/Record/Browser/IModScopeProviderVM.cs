using System.Collections.ObjectModel;
using CreationEditor.Avalonia.Models.Mod;
using CreationEditor.Services.Filter;
namespace CreationEditor.Avalonia.ViewModels.Record.Browser;

public interface IModScopeProviderVM : IModScopeProvider {
    new ReadOnlyObservableCollection<ModItem> Mods { get; }
}
