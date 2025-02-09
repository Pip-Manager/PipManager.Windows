using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using HotAvalonia;
using Microsoft.Extensions.DependencyInjection;
using PipManager.Core.Configuration;
using PipManager.Desktop.Languages;
using PipManager.Desktop.Services;
using PipManager.Desktop.ViewModels;
using PipManager.Desktop.Views;
using Serilog;

namespace PipManager.Desktop;

public class App : Application
{
    public override void Initialize()
    {
        this.EnableHotReload();
        AvaloniaXamlLoader.Load(this);
        
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", AppInfo.AppVersion)
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(Path.Combine(AppInfo.LogDirectoryPath, "log_.txt"), outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();
        Log.Information("Logging started");
        
        LoadConfig();
    }

    private static void LoadConfig()
    {
        if (Configuration.Initialize(AppInfo.AppDirectoryPath))
        {
            return;
        }

        Log.Error($"[{nameof(Configuration)}] Config file is broken");
    }

    private static IServiceProvider Services { get; } = 
        new ServiceCollection()
        .AddViews()
        .AddViewModels()
        .AddServices()
        .BuildServiceProvider();
    
    internal static T GetService<T>() where T : notnull => (Services ?? throw new InvalidOperationException("Services not Configured")).GetRequiredService<T>();
    internal static object GetService(Type type) => (Services ?? throw new InvalidOperationException("Services not Configured")).GetRequiredService(type);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.DataContext = Services.GetRequiredService<MainWindowViewModel>();
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
    // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
    private static void DisableAvaloniaDataAnnotationValidation()
    {
        foreach (var plugin in BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray())
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}