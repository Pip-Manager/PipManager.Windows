using System.Text.Json.Serialization;

namespace PipManager.Core.PyPackage.Models;

public class PackageInfo
{
    [JsonPropertyName("releases")]
    public Dictionary<string, List<PackageInfoRelease>>? Releases { get; init; }
}

public class PackageInfoRelease
{
    [JsonPropertyName("upload_time")]
    public string? UploadTime { get; set; }
}