using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using Serilog;

namespace PipManager.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private bool _experimentMode;

    public MainWindowViewModel(IConfigurationService configurationService, IEnvironmentService environmentService)
    {

        if (configurationService.AppConfig.CurrentEnvironment != null)
        {
            Log.Information($"[MainWindow] Environment loaded ({configurationService.AppConfig.CurrentEnvironment.PipVersion} for {configurationService.AppConfig.CurrentEnvironment.PythonVersion})");
            ApplicationTitle = $"Pip Manager | {configurationService.AppConfig.CurrentEnvironment.PipVersion} for {configurationService.AppConfig.CurrentEnvironment.PythonVersion}";
        }
        else
        {
            Log.Information("[MainWindow] No previous selected environment found");
            ApplicationTitle = "Pip Manager";
        }
    }
    
    [ObservableProperty]
    private bool _isTitleBarCoverageGridVisible;

    [ObservableProperty]
    private string _applicationTitle = "Pip Manager";
}