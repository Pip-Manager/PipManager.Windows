using PipManager.Models;
using PipManager.Models.AppConfigModels;

namespace PipManager.Services.Environment;

public interface IEnvironmentService
{
    public AppConfig AppConfig { get; set; }
    public void Initialize(AppConfig appConfig);
    public bool CheckEnvironmentExists(EnvironmentItem environmentItem);
    public EnvironmentItem? GetEnvironmentItemFromCommand(string command, string arguments);
    public (bool, string) CheckEnvironmentAvailable(EnvironmentItem environmentItem);
}