using PipManager.Models;
using PipManager.Models.AppConfigModels;
using PipManager.Models.Package;

namespace PipManager.Services.Configuration;

public interface IConfigurationService
{
    public AppConfig AppConfig { get; set; }
    public bool DebugMode { get; set; }
    public bool ExperimentMode { get; set; }

    public string GetUrlFromPackageSourceType(string index = "simple");

    public string GetTestingUrlFromPackageSourceType(PackageSourceType packageSourceType);

    public void Save();

    public string FindPythonPathByPipDir(string pipDir);

    public EnvironmentItem? GetEnvironmentItem(string pythonPath);

    public EnvironmentItem? GetEnvironmentItemFromCommand(string command, string arguments);

    public void RefreshAllEnvironmentVersions();
}