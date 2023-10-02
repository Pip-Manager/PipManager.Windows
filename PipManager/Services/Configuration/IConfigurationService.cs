using PipManager.Models;
using PipManager.Models.AppConfigModels;

namespace PipManager.Services.Configuration;

public interface IConfigurationService
{
    public AppConfig AppConfig { get; set; }

    public void Initialize();

    public bool CheckEnvironmentExists(EnvironmentItem environmentItem);

    public EnvironmentItem? GetEnvironmentItemFromCommand(string command, string arguments);

    public (bool, string) CheckEnvironmentAvailable(EnvironmentItem environmentItem);

    public string GetUrlFromPackageSourceType(PackageSourceType packageSourceType);

    public string GetTestingUrlFromPackageSourceType(PackageSourceType packageSourceType);

    public void Save();
}