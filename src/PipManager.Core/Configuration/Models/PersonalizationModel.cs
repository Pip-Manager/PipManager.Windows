using System.Text.Json.Serialization;

namespace PipManager.Core.Configuration.Models;

[JsonSerializable(typeof(PersonalizationModel))]
public partial class PersonalizationModelContext : JsonSerializerContext;
public class PersonalizationModel
{
    [JsonPropertyName("language")]
    public string Language { get; set; } = "Auto";

    [JsonPropertyName("theme")]
    public string Theme { get; set; } = "dark";
}