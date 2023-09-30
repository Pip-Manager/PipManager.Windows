using System.IO;

namespace PipManager;

public static class AppInfo
{
    public static readonly string AppVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                                                   ?? string.Empty;

    public static readonly string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    public static readonly string CrushesDir = Path.Combine(Directory.GetCurrentDirectory(), "crashes");
    public static readonly string LogDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
}