using Newtonsoft.Json;

namespace PipManager.Models.PipInspection;

public class PipInspection
{
    [JsonProperty("pip_version")] public string PipVersion { get; set; }
    [JsonProperty("installed")] public List<PipMetadata> Installed { get; set; }
}

public class PipMetadata
{
    [JsonProperty("metadata")] public PipInfo Information { get; set; }
    [JsonProperty("metadata_location")] public string Location { get; set; }
}

public class PipInfo
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("version")] public string Version { get; set; }
    [JsonProperty("summary")] public string Summary { get; set; }
    [JsonProperty("requires_python")] public string? RequiresPython { get; set; }
    [JsonProperty("project_url")] public List<string>? ProjectUrlList { get; set; }
}