using PipManager.Core.PyPackage.Models;
using PipManager.Core.Wrappers.PackageSearchIndexWrapper;

namespace PipManager.Core.Services.PackageSearchService;

public interface IPackageSearchService
{
    public Task<List<IndexItemModel>?> GetIndexAsync(PackageSourceType packageSourceType);
}