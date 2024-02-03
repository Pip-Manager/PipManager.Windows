using PipManager.Models.AppConfigModels;
using PipManager.Models.Package;
using PipManager.Services.Environment.Response;
using System.Diagnostics;

namespace PipManager.Services.Environment;

public interface IEnvironmentService
{
    public bool CheckEnvironmentExists(EnvironmentItem environmentItem);

    public (bool, string) CheckEnvironmentAvailable(EnvironmentItem environmentItem);

    public List<PackageItem>? GetLibraries();

    public Task<GetVersionsResponse> GetVersions(string packageName);

    public (bool, string) Install(string packageName, DataReceivedEventHandler consoleOutputCallback);
    public (bool, string) InstallByRequirements(string requirementsFilePath, DataReceivedEventHandler consoleOutputCallback);

    public (bool, string) Update(string packageName, DataReceivedEventHandler consoleOutputCallback);

    public (bool, string) Uninstall(string packageName, DataReceivedEventHandler consoleOutputCallback);
}