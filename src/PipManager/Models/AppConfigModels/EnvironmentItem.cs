using Newtonsoft.Json;

namespace PipManager.Models.AppConfigModels;

public class EnvironmentItem
{
    public EnvironmentItem()
    {
    }

    public EnvironmentItem(string pipVersion, string pythonPath, string pythonVersion, string pythonDllPath)
    {
        PipVersion = pipVersion;
        PythonPath = pythonPath;
        PythonVersion = pythonVersion;
        PythonDllPath = pythonDllPath;
    }

    [JsonProperty("pipVersion")] public string? PipVersion { get; set; }
    [JsonProperty("pythonPath")] public string? PythonPath { get; set; }
    [JsonProperty("pythonVersion")] public string? PythonVersion { get; set; }
    [JsonProperty("pythonDllPath")] public string? PythonDllPath { get; set; }
}