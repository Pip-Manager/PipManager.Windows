using PipManager.Models;
using PipManager.Models.AppConfigModels;
using PipManager.Models.PipInspection;

namespace PipManager.Services.Environment;

public interface IEnvironmentService
{

    public bool CheckEnvironmentExists(EnvironmentItem environmentItem);

    public EnvironmentItem? GetEnvironmentItemFromCommand(string command, string arguments);

    public (bool, string) CheckEnvironmentAvailable(EnvironmentItem environmentItem);
    public List<PipMetadata>? GetLibraries();
}