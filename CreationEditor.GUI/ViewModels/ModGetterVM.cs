using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog.WPF;
namespace CreationEditor.GUI.ViewModels; 

public interface IModGetterVM {
    public bool IsReadOnly { get; set; }
    
    public string Name { get; set; }
    public ModType Type { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public bool Localization { get; set; }
    public int FormVersion { get; set; }
    public List<string> Masters { get; set; }
}

public class SkyrimModGetterVM : ViewModel, IModGetterVM {
    public bool IsReadOnly { get; set; } = true;
    
    public string Name { get; set; }
    public ModType Type { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public bool Localization { get; set; }
    public int FormVersion { get; set; }
    public List<string> Masters { get; set; }

    public SkyrimModGetterVM(ModKey modKey, ISkyrimModHeaderGetter header) {
        Name = modKey.Name;
        Type = modKey.Type;

        Author = header.Author ?? string.Empty;
        Description = header.Description ?? string.Empty;
        
        Localization = (header.Flags & SkyrimModHeader.HeaderFlag.Localized) != 0;
        FormVersion = header.FormVersion;
        Masters = header.MasterReferences.Select(master => master.Master.FileName.String).ToList();
    }
}
