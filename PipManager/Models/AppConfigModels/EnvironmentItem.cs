using Newtonsoft.Json;

namespace PipManager.Models.AppConfigModels;

public class EnvironmentItem
{
    public EnvironmentItem(string pipVersion, string pipDir, string pythonVersion)
    {
        PipVersion = pipVersion;
        PipDir = pipDir;
        PythonVersion = pythonVersion;
    }

    public EnvironmentItem()
    {
        PipVersion = "";
        PipDir = "";
        PythonVersion = "";
    }

    [JsonProperty("pipVersion")] public string PipVersion { get; set; } = string.Empty;
    [JsonProperty("pipDir")] public string PipDir { get; set; } = string.Empty;
    [JsonProperty("pythonVersion")] public string PythonVersion { get; set; } = string.Empty;
}