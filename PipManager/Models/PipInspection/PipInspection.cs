using Newtonsoft.Json;

namespace PipManager.Models.PipInspection;


public class PipInspection
{
    [JsonProperty("pip_version")] public required string PipVersion { get; set; }
    [JsonProperty("installed")] public required List<PipMetadata> Installed { get; set; }
}

public class PipMetadata
{
    [JsonProperty("metadata")] public required PipInfo Information { get; set; }
    [JsonProperty("metadata_location")] public required string Location { get; set; }
}

public class PipInfo
{
    [JsonProperty("name")] public required string Name { get; set; }
    [JsonProperty("version")] public required string Version { get; set; }
    [JsonProperty("summary")] public required string Summary { get; set; }
    [JsonProperty("requires_python")] public string? RequiresPython { get; set; }
    [JsonProperty("project_url")] public List<string>? ProjectUrlList { get; set; }
}