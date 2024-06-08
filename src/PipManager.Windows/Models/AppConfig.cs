using Newtonsoft.Json;
using PipManager.Windows.Models.AppConfigModels;

namespace PipManager.Windows.Models;

public class AppConfig
{
    [JsonProperty("currentEnvironment")] public EnvironmentItem? CurrentEnvironment { get; set; }
    [JsonProperty("environments")] public List<EnvironmentItem> EnvironmentItems { get; set; } = [];
    [JsonProperty("packageSource")] public PackageSource PackageSource { get; set; } = new();
    [JsonProperty("personalization")] public Personalization Personalization { get; set; } = new();
}