using PipManager.PackageSearch.Wrappers.Query;

namespace PipManager.PackageSearch;

public interface IPackageSearchService
{
    public Task<QueryWrapper> Query(string name, int page=1);
}