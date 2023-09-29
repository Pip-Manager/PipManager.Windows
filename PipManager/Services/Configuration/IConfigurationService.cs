using PipManager.Models;

namespace PipManager.Services.Configuration;

public interface IConfigurationService
{
    public AppConfig AppConfig { get; set; }
    public void Initialize();

    public void Save();
}