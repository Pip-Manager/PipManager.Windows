using Serilog;
using System.Globalization;
using System.IO;
using Windows.Win32;
using PipManager.Core.Configuration;
using PipManager.Windows.Languages;

namespace PipManager.Windows;

public static class AppStarting
{
    public static void LoadConfig()
    {
        if (Configuration.Initialize(AppInfo.ConfigDirectory))
        {
            return;
        }

        var uiMessageBox = new Wpf.Ui.Controls.MessageBox
        {
            Title = Lang.ContentDialog_Title_Warning,
            Content = Lang.ContentDialog_Message_BrokenConfig,
            PrimaryButtonText = Lang.ContentDialog_PrimaryButton_ResetConfig,
            CloseButtonText = Lang.ContentDialog_CloseButton_Cancel
        };

        if (uiMessageBox.ShowDialogAsync().Result == Wpf.Ui.Controls.MessageBoxResult.Primary)
        {
            Configuration.Initialize(AppInfo.ConfigDirectory, true);
        }
        else
        {
            Environment.Exit(0);
        }
    }

    public static void CreateDirectories()
    {
        Directory.CreateDirectory(AppInfo.CrushesDir);
        Directory.CreateDirectory(AppInfo.LogDir);
        Directory.CreateDirectory(AppInfo.CachesDir);
    }

    public static void LoadLanguage()
    {
        var language = Configuration.AppConfig.Personalization.Language;
        if (language != "Auto")
        {
            I18NExtension.Culture = new CultureInfo(language);
        }
        Log.Information($"Language sets to {language}");
    }

    public static void StartLogging(bool showConsoleWindow)
    {
        if (showConsoleWindow)
        {
            PInvoke.AllocConsole();
        }
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", AppInfo.AppVersion)
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(Path.Combine(AppInfo.LogDir, "log_.txt"), outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();
        Log.Information("Logging started");
    }

    public static void CachesDeletion()
    {
        if (!Directory.Exists(AppInfo.CachesDir)) return;
        var directoryInfo = new DirectoryInfo(AppInfo.CachesDir);
        var filesInfo = directoryInfo.GetFileSystemInfos();
        var cacheFileAmount = 0;
        foreach (var subDir in directoryInfo.GetDirectories("tempTarGz-*", SearchOption.AllDirectories))
        {
            try
            {
                subDir.Delete(true);
            }
            catch
            {
                Log.Warning("Failed to delete cache directory: {DirFullName}", subDir.FullName);
            }
        }
        foreach (var file in filesInfo)
        {
            if (!file.Name.StartsWith("temp_"))
            {
                continue;
            }

            try
            {
                File.Delete(file.FullName);
                cacheFileAmount++;
            }
            catch
            {
                Log.Warning("Failed to delete cache file: {FileFullName}", file.FullName);
            }
        }
        Log.Information($"{cacheFileAmount} cache file(s) deleted");
    }
}