using PipManager.Core.Configuration.Models;
using PipManager.Windows.Models.Package;

namespace PipManager.Windows.Services.Configuration;

public interface IConfigurationService
{
    public ConfigModel AppConfig { get; set; }
    public bool DebugMode { get; set; }
    public bool ExperimentMode { get; set; }

    public string GetUrlFromPackageSourceType(string index = "simple");

    public string GetTestingUrlFromPackageSourceType(PackageSourceType packageSourceType);

    public void Save();

    public string FindPythonPathByPipDir(string pipDir);

    public EnvironmentModel? GetEnvironmentItem(string pythonPath);

    public EnvironmentModel? GetEnvironmentItemFromCommand(string command, string arguments);

    public void RefreshAllEnvironmentVersions();
}