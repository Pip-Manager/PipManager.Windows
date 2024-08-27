using Serilog;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using PipManager.Core.Configuration;

namespace PipManager.Windows;

public partial class AppStarting
{
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial void AllocConsole();

    public bool ShowConsoleWindow = false;

    public AppStarting()
    {
        Configuration.Initialize(AppInfo.ConfigDirectory);
        Directory.CreateDirectory(AppInfo.CrushesDir);
        Directory.CreateDirectory(AppInfo.LogDir);
        Directory.CreateDirectory(AppInfo.CachesDir);
    }

    public void LoadLanguage()
    {
        var language = Configuration.AppConfig!.Personalization.Language;
        if (language != "Auto")
        {
            I18NExtension.Culture = new CultureInfo(language);
        }
        Log.Information($"Language sets to {language}");
    }

    public void StartLogging()
    {
        if (ShowConsoleWindow)
        {
            AllocConsole();
        }
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", AppInfo.AppVersion)
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(Path.Combine(AppInfo.LogDir, "log_.txt"), outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();
        Log.Information("Logging started");
    }

    public void CachesDeletion()
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