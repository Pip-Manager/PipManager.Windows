using Newtonsoft.Json;
using PipManager.Models;
using System.IO;

namespace PipManager.Services.Configuration;

public class ConfigurationService : IConfigurationService
{
    public AppConfig AppConfig { get; set; }

    public static AppConfig LoadConfiguration()
    {
        if (!File.Exists(AppInfo.ConfigPath))
        {
            File.WriteAllText(AppInfo.ConfigPath, JsonConvert.SerializeObject(new AppConfig(), Formatting.Indented));
        }
        return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(AppInfo.ConfigPath))!;
    }

    public void Initialize()
    {
        AppConfig = LoadConfiguration();
    }

    public void Save()
    {
        File.WriteAllText(AppInfo.ConfigPath, JsonConvert.SerializeObject(AppConfig, Formatting.Indented));
    }
}