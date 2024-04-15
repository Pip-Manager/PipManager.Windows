using PipManager.PackageSearch.Wrappers.Query;

namespace PipManager.PackageSearch;

public interface IPackageSearchService
{
    public ValueTask<QueryWrapper> Query(string name, int page = 1);
}