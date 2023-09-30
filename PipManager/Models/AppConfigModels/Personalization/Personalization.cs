using Newtonsoft.Json;

namespace PipManager.Models.AppConfigModels.Personalization;

public class Personalization
{
    [JsonProperty("language")]
    public string Language { get; set; } = "Auto";
    [JsonProperty("theme")]
    public string Theme { get; set; } = "dark";
    
    [JsonProperty("logAutoDeletion")]
    public bool LogAutoDeletion { get; set; } = true;
    
    [JsonProperty("logAutoDeletionTimes")]
    public int LogAutoDeletionTimes { get; set; } = 7;
    
    [JsonProperty("crushesAutoDeletion")]
    public bool CrushesAutoDeletion { get; set; } = true;
    
    [JsonProperty("crushesAutoDeletionTimes")]
    public int CrushesAutoDeletionTimes { get; set; } = 7;
}