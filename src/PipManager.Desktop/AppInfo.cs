using System.IO;
using System.Reflection;

namespace PipManager.Desktop;

internal static class AppInfo
{
    internal static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);
    internal static readonly string AppDirectoryPath = Directory.GetCurrentDirectory();
    
    internal static readonly string CrushesDirectoryPath = Path.Combine(AppDirectoryPath, "crashes");
    internal static readonly string LogDirectoryPath = Path.Combine(AppDirectoryPath, "logs");
    internal static readonly string CachesDirectoryPath = Path.Combine(AppDirectoryPath, "caches");
}