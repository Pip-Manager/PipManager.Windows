using Newtonsoft.Json;

namespace PipManager.Models.AppConfigModels;

public class PackageSource
{
    [JsonProperty("packageSourceType")] public string PackageSourceType { get; set; } = "Official";
}