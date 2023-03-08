﻿using Autofac;
using CreationEditor.Avalonia.ViewModels.Logging;
using Serilog;
using Serilog.Events;
namespace CreationEditor.Avalonia.Modules;

public sealed class LoggingModule : Module {
    protected override void Load(ContainerBuilder builder) {
        const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";

        var logSink = new ReplayLogSink();
        builder.RegisterInstance(logSink)
            .As<IObservableLogSink>();

        builder.RegisterType<LogVM>()
            .As<ILogVM>();

        var logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Sink(logSink)
            .WriteTo.Console(LogEventLevel.Verbose, outputTemplate)
            .WriteTo.File("log.txt", LogEventLevel.Verbose, outputTemplate)
            .CreateLogger();

        builder.RegisterInstance(logger)
            .As<ILogger>()
            .SingleInstance();

    }
}
