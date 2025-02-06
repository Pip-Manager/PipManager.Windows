using System.Text.Json.Serialization;

namespace PipManager.Core.Configuration.Models;

[JsonSerializable(typeof(EnvironmentModel))]
public partial class EnvironmentModelContext : JsonSerializerContext;

public class EnvironmentModel
{
    [JsonPropertyName("identifier")]
    public required string Identifier { get; set; }
    [JsonPropertyName("pythonPath")]
    public required string PythonPath { get; set; }
    [JsonPropertyName("pipVersion")]
    public required string PipVersion { get; set; }
    [JsonPropertyName("pythonVersion")]
    public required string PythonVersion { get; set; }
}