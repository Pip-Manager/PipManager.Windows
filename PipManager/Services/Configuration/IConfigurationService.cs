using PipManager.Models;

namespace PipManager.Services.Configuration;

public interface IConfigurationService
{
    public AppConfig AppConfig { get; set; }

    public string GetUrlFromPackageSourceType(PackageSourceType packageSourceType);

    public string GetTestingUrlFromPackageSourceType(PackageSourceType packageSourceType);

    public void Save();
}