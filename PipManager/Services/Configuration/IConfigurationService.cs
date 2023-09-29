using PipManager.Models;

namespace PipManager.Services.Configuration;

public interface IConfigurationService
{
    public string AppVersion { get; }

    public AppConfig AppConfig { get; set; }
    public void Initialize();

    public void Save();
}