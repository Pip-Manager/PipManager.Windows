using PipManager.Models.AppConfigModels;
using PipManager.Models.PipInspection;

namespace PipManager.Services.Environment;

public interface IEnvironmentService
{
    public bool CheckEnvironmentExists(EnvironmentItem environmentItem);

    public (bool, string) CheckEnvironmentAvailable(EnvironmentItem environmentItem);

    public List<PipMetadata>? GetLibraries();
    public string[] GetVersions(string packageName);
    public (bool, string) Update(string packageName);

    public (bool, string) Uninstall(string packageName);
}