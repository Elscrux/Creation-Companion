using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Autofac;
using CreationEditor.GUI.Modules;
using CreationEditor.GUI.Services.Startup;
using CreationEditor.GUI.Views.Windows;
using Elscrux.Notification;
using Microsoft.Win32;
using Syncfusion.SfSkinManager;
using Syncfusion.Themes.MaterialDark.WPF;
using Syncfusion.Themes.MaterialLight.WPF;
namespace CreationEditor.GUI;

public partial class App {
    private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    private const string RegistryValueName = "AppsUseLightTheme";
    
    public App() {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnFirstChanceException;
    }
    
    private static void CurrentDomainOnFirstChanceException(object sender, UnhandledExceptionEventArgs e) {
        var exception = (Exception) e.ExceptionObject;
        
        using var log = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CrashLog.txt"), false);
        log.WriteLine(exception);
    }

    protected override void OnStartup(StartupEventArgs e) {
        base.OnStartup(e);
        
        var window = new MainWindow();

        var builder = new ContainerBuilder();
        
        builder.RegisterModule<MainModule>();
        builder.RegisterModule<NotificationModule>();
        builder.RegisterModule<LoggingModule>();
        // builder.RegisterModule<MutagenModule>();
        builder.RegisterModule<SkyrimModule>();
        
        builder.RegisterInstance(window).As<IMainWindow>();
        
        var container = builder.Build();
        
        container.Resolve<IStartup>()
            .Start();

        window.Show();
    }

    private enum WindowsTheme { Light, Dark }

    private static WindowsTheme GetTheme() {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);
        var registryValueObject = key?.GetValue(RegistryValueName);
        if (registryValueObject == null) return WindowsTheme.Light;

        var registryValue = (int) registryValueObject;
        return registryValue == 1 ? WindowsTheme.Light : WindowsTheme.Dark;
    }
    
    public static void UpdateTheme(DependencyObject dependencyObject) {
        // SfSkinManager.ApplyStylesOnApplication = true;
        if (GetTheme() == WindowsTheme.Dark) {
            var darkThemeSettings = new MaterialDarkThemeSettings {
                PrimaryBackground = new SolidColorBrush(SystemParameters.WindowGlassColor)
            };
            SfSkinManager.RegisterThemeSettings("MaterialDark", darkThemeSettings);
            SfSkinManager.SetTheme(dependencyObject, new Theme("MaterialDark"));

        } else {
            var lightThemeSettings = new MaterialLightThemeSettings {
                PrimaryBackground = new SolidColorBrush(SystemParameters.WindowGlassColor)
            };
            SfSkinManager.RegisterThemeSettings("MaterialLight", lightThemeSettings);
            SfSkinManager.SetTheme(dependencyObject, new Theme("MaterialLight"));
        }
    }
}