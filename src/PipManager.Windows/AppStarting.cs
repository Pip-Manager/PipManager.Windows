using PipManager.Windows.Services.Configuration;
using Serilog;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using PipManager.Windows.Helpers;
using PipManager.Windows.Models;

namespace PipManager.Windows;

public partial class AppStarting
{
    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial void AllocConsole();

    private readonly AppConfig _config;
    public bool ShowConsoleWindow = false;

    public AppStarting()
    {
        _config = ConfigurationService.LoadConfiguration();
        Directory.CreateDirectory(AppInfo.CrushesDir);
        Directory.CreateDirectory(AppInfo.LogDir);
        Directory.CreateDirectory(AppInfo.CachesDir);
    }

    public void LoadLanguage()
    {
        var language = _config.Personalization.Language;
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
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("Version", AppInfo.AppVersion)
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] (Thread: {ThreadId}) {Message}{NewLine}{Exception}")
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.File(Path.Combine(AppInfo.LogDir, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt"), outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] (Thread: {ThreadId}) {Message}{NewLine}{Exception}")
                .CreateLogger();
        }
        else
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("Version", AppInfo.AppVersion)
                .MinimumLevel.Debug()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.File(Path.Combine(AppInfo.LogDir, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt"), outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] (Thread: {ThreadId}) {Message}{NewLine}{Exception}")
                .CreateLogger();
        }
        Log.Information("Logging started");
    }

    public void LogDeletion()
    {
        if (!_config.Personalization.LogAutoDeletion || !Directory.Exists(AppInfo.LogDir)) return;
        var fileList = Directory.GetFileSystemEntries(AppInfo.LogDir);
        var logFileAmount = fileList.Count(file => File.Exists(file) && file.EndsWith(".txt"));
        if (logFileAmount < _config.Personalization.LogAutoDeletionTimes)
        {
            return;
        }

        var directoryInfo = new DirectoryInfo(AppInfo.LogDir);
        var filesInfo = directoryInfo.GetFileSystemInfos();
        foreach (var file in filesInfo)
        {
            if (file.Extension != ".txt") continue;
            try
            {
                File.Delete(file.FullName);
            }
            catch
            {
                Log.Warning("Failed to delete log: {FileFullName}", file.FullName);
            }
        }
        Log.Information($"{logFileAmount} log file(s) deleted");
    }

    public void CrushesDeletion()
    {
        if (!_config.Personalization.CrushesAutoDeletion || !Directory.Exists(AppInfo.CrushesDir)) return;
        var fileList = Directory.GetFileSystemEntries(AppInfo.CrushesDir);
        var crushFileAmount = fileList.Count(file => File.Exists(file) && file.EndsWith(".txt"));
        if (crushFileAmount < _config.Personalization.CrushesAutoDeletionTimes)
        {
            return;
        }

        var directoryInfo = new DirectoryInfo(AppInfo.CrushesDir);
        var filesInfo = directoryInfo.GetFileSystemInfos();
        foreach (var file in filesInfo)
        {
            if (file.Extension != ".txt") continue;
            try
            {
                File.Delete(file.FullName);
            }
            catch
            {
                Log.Warning("Failed to delete crush file: {FileFullName}", file.FullName);
            }
        }
        Log.Information($"{crushFileAmount} crush file(s) deleted");
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