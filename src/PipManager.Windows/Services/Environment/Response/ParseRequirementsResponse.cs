namespace PipManager.Windows.Services.Environment.Response;

public class ParseRequirementsResponse
{
    public bool Success { get; init; }
    public List<ParsedRequirement>? Requirements { get; init; }
}

public class ParsedRequirement
{
    public required string Name { get; init; }
    public required string Specifier { get; init; }
}
