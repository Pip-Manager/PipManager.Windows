using Newtonsoft.Json;

namespace PipManager.Models.AppConfigModels;

public class EnvironmentItem
{
    public EnvironmentItem()
    {
    }

    public EnvironmentItem(string pipVersion, string pythonPath, string pythonVersion)
    {
        PipVersion = pipVersion;
        PythonPath = pythonPath;
        PythonVersion = pythonVersion;
    }

    [JsonProperty("pipVersion")] public string PipVersion { get; set; } = string.Empty;
    [JsonProperty("pythonPath")] public string PythonPath { get; set; } = string.Empty;
    [JsonProperty("pythonVersion")] public string PythonVersion { get; set; } = string.Empty;
}