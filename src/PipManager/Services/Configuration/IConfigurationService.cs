using PipManager.Models;
using PipManager.Models.AppConfigModels;

namespace PipManager.Services.Configuration;

public interface IConfigurationService
{
    public AppConfig AppConfig { get; set; }

    public string GetUrlFromPackageSourceType(string index = "simple");

    public string GetUrlFromPackageSourceType(PackageSourceType packageSourceType, string index="simple");

    public string GetTestingUrlFromPackageSourceType(PackageSourceType packageSourceType);

    public void Save();

    public string FindPythonPathByPipDir(string pipDir);

    public EnvironmentItem? GetEnvironmentItemFromCommand(string command, string arguments);

    public Task RefreshAllEnvironmentVersions();
}