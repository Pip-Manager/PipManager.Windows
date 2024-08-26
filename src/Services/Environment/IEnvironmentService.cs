using System.Diagnostics;
using PipManager.Core.Configuration.Models;
using PipManager.Windows.Models.Package;
using PipManager.Windows.Services.Environment.Response;

namespace PipManager.Windows.Services.Environment;

public interface IEnvironmentService
{
    public bool CheckEnvironmentExists(EnvironmentModel environmentModel);

    public ActionResponse CheckEnvironmentAvailable(EnvironmentModel environmentModel);

    public ActionResponse PurgeEnvironmentCache(EnvironmentModel environmentModel);

    public Task<List<PackageItem>?> GetLibraries();

    public Task<GetVersionsResponse> GetVersions(string packageName, CancellationToken cancellationToken, bool detectNonRelease = true);
    public bool TryKillProcess();

    public ActionResponse Install(string packageName, DataReceivedEventHandler consoleOutputCallback, string[]? extraParameters = null);

    public ActionResponse InstallByRequirements(string requirementsFilePath, DataReceivedEventHandler consoleOutputCallback);

    public ActionResponse Download(string packageName, string downloadPath, DataReceivedEventHandler consoleOutputCallback, string[]? extraParameters = null);

    public ActionResponse Update(string packageName, DataReceivedEventHandler consoleOutputCallback);

    public ActionResponse Uninstall(string packageName, DataReceivedEventHandler consoleOutputCallback);
}