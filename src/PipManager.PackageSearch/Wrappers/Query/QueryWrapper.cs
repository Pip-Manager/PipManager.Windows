namespace PipManager.PackageSearch.Wrappers.Query;

public class QueryWrapper
{
    public QueryStatus Status { get; set; }
    public string? ResultCount { get; set; }
    public List<QueryListItemModel>? Results { get; set;}
    public int MaxPageNumber { get; set; }
}

