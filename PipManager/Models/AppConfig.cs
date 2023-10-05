using Newtonsoft.Json;
using PipManager.Models.AppConfigModels;

namespace PipManager.Models;

public class AppConfig
{
    [JsonProperty("currentEnvironment")] public EnvironmentItem CurrentEnvironment { get; set; } = new();
    [JsonProperty("environments")] public List<EnvironmentItem> EnvironmentItems { get; set; } = new();
    [JsonProperty("packageSource")] public PackageSource PackageSource { get; set; } = new();
    [JsonProperty("personalization")] public Personalization Personalization { get; set; } = new();
}