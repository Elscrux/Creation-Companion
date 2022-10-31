using Autofac;
using Elscrux.WPF.ViewModels;
using Serilog;
using Serilog.Events;
namespace CreationEditor.WPF.Modules;

public class LoggingModule : Module {
    protected override void Load(ContainerBuilder builder) {
        const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";

        var logVM = new LogVM();
        builder.RegisterInstance(logVM)
            .As<ILogVM>();
        
        var logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Sink(logVM)
            .WriteTo.Console(LogEventLevel.Verbose, outputTemplate)
            .WriteTo.File("log.txt", LogEventLevel.Verbose, outputTemplate)
            .CreateLogger();

        builder.RegisterInstance(logger)
            .As<ILogger>()
            .SingleInstance();

    }
}
