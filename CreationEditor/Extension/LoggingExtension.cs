using System.Runtime.CompilerServices;
using Serilog;
namespace CreationEditor.Extension;

public static class LoggingExtension {
    private const string MemberName = "MemberName";
    private const string FilePath = "FilePath";
    private const string LineNumber = "LineNumber";

    public static ILogger Here(this ILogger logger,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0) {
        return logger
            .ForContext(MemberName, memberName)
            .ForContext(FilePath, sourceFilePath)
            .ForContext(LineNumber, sourceLineNumber);
    }
}
