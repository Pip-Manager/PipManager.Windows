using PipManager.Services.Configuration;
using Serilog;

namespace PipManager.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IConfigurationService _configurationService;
    [ObservableProperty] private bool _experimentMode;

    public MainWindowViewModel(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
        if (_configurationService.AppConfig.CurrentEnvironment != null)
        {
            Log.Information($"[MainWindow] Environment loaded ({_configurationService.AppConfig.CurrentEnvironment.PipVersion} for {_configurationService.AppConfig.CurrentEnvironment.PythonVersion})");
            ApplicationTitle = $"Pip Manager | {_configurationService.AppConfig.CurrentEnvironment.PipVersion} for {_configurationService.AppConfig.CurrentEnvironment.PythonVersion}";
        }
        else
        {
            Log.Information("[MainWindow] No previous selected environment found");
            ApplicationTitle = "Pip Manager";
        }
    }

    [ObservableProperty]
    private string _applicationTitle = "Pip Manager";
}