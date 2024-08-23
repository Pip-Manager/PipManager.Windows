using Newtonsoft.Json;
using PipManager.Windows.Models.Package;

namespace PipManager.Windows.Models.AppConfigModels;

public class PackageSource
{
    [JsonProperty("packageSourceType")] public PackageSourceType PackageSourceType { get; set; } = PackageSourceType.Official;
    [JsonProperty("detectNonReleaseVersion")] public bool DetectNonReleaseVersion { get; set; }
}