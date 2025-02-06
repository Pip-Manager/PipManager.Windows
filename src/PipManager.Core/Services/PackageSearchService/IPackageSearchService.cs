using PipManager.Core.Wrappers.PackageSearchQueryWrapper;

namespace PipManager.Core.Services.PackageSearchService;

public interface IPackageSearchService
{
    public ValueTask<QueryWrapper> Query(string name, int page = 1);
}