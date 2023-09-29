using System.IO;
using Newtonsoft.Json;
using PipManager.Models;

namespace PipManager.Services.Configuration;

public class ConfigurationService: IConfigurationService
{
    public readonly string ConfigPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
    public readonly string CrushesDir = Path.Combine(Directory.GetCurrentDirectory(), "crashes");
    public readonly string LogDir = Path.Combine(Directory.GetCurrentDirectory(), "logs");
    
    public string AppVersion { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
                                                       ?? string.Empty;
    public required AppConfig AppConfig { get; set; }
    public void Initialize()
    {
        if (!File.Exists(ConfigPath))
        {
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(new AppConfig(), Formatting.Indented));
        }
        AppConfig = JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(ConfigPath));
    }
    
    public void Save()
    {
        File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(AppConfig, Formatting.Indented));
    }
}