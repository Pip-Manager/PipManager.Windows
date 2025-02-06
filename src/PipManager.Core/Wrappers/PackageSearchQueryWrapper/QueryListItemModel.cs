namespace PipManager.Core.Wrappers.PackageSearchQueryWrapper;

public class QueryListItemModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string Url { get; set; }
    public DateTime UpdateTime { get; set; }
}