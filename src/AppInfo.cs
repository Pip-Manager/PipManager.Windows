using System.IO;
using System.Reflection;

namespace PipManager.Windows;

public static class AppInfo
{
    public static readonly string AppVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);

    public static readonly string ConfigDirectory = Directory.GetCurrentDirectory();
    public static readonly string CrushesDir = Path.Combine(Directory.GetCurrentDirectory(), "crashes");
    public static readonly string LogDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
    public static readonly string CachesDir = Path.Combine(Directory.GetCurrentDirectory(), "caches");
}