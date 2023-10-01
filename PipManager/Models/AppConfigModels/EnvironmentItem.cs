using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace PipManager.Models.AppConfigModels;

public class EnvironmentItem
{
    public EnvironmentItem(string description, string pipVersion, string pipDir, string pythonVersion)
    {
        Description = description;
        PipVersion = pipVersion;
        PipDir = pipDir;
        PythonVersion = pythonVersion;
    }

    [JsonProperty("description")] public string Description { get; set; } = string.Empty;
    [JsonProperty("pipVersion")] public string PipVersion { get; set; } = string.Empty;
    [JsonProperty("pipDir")] public string PipDir { get; set; } = string.Empty;
    [JsonProperty("pythonVersion")] public string PythonVersion { get; set; } = string.Empty;
}