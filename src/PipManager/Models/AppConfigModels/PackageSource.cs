using Newtonsoft.Json;

namespace PipManager.Models.AppConfigModels;

public class PackageSource
{
    [JsonProperty("packageSourceType")] public PackageSourceType PackageSourceType { get; set; } = PackageSourceType.Official;
}