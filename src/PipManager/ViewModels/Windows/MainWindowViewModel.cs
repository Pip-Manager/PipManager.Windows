using PipManager.Languages;
using PipManager.Services.Configuration;
using PipManager.Views.Pages.About;
using PipManager.Views.Pages.Action;
using PipManager.Views.Pages.Environment;
using PipManager.Views.Pages.Library;
using PipManager.Views.Pages.Search;
using PipManager.Views.Pages.Settings;
using PipManager.Views.Pages.Tools;
using Serilog;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IConfigurationService _configurationService;

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