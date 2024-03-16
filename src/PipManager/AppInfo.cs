using System.IO;
using System.Reflection;

namespace PipManager;

public static class AppInfo
{
    public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3) ?? string.Empty;

    public static readonly string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    public static readonly string CrushesDir = Path.Combine(Directory.GetCurrentDirectory(), "crashes");
    public static readonly string LogDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
    public static readonly string CachesDir = Path.Combine(Directory.GetCurrentDirectory(), "caches");
}