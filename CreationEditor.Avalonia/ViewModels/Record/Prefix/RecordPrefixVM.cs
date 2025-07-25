using CreationEditor.Avalonia.Services.Record.Prefix;
namespace CreationEditor.Avalonia.ViewModels.Record.Prefix;

public class RecordPrefixVM(IRecordPrefixService recordPrefixService) : ViewModel {
    public IRecordPrefixService RecordPrefixService { get; } = recordPrefixService;
}
