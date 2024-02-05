namespace PipManager.PackageSearch.Wrappers.Query;

public class QueryListItemModel
{
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string Description { get; set; }
    public DateTime UpdateTime { get; set; }
}