﻿using PipManager.Windows.Services.Configuration;
using Serilog;

namespace PipManager.Windows.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private bool _experimentMode;

    public MainWindowViewModel(IConfigurationService configurationService)
    {

        if (configurationService.AppConfig.SelectedEnvironment != null)
        {
            Log.Information($"[MainWindow] Environment loaded ({configurationService.AppConfig.SelectedEnvironment.PipVersion} for {configurationService.AppConfig.SelectedEnvironment.PythonVersion})");
            ApplicationTitle = $"Pip Manager | {configurationService.AppConfig.SelectedEnvironment.PipVersion} for {configurationService.AppConfig.SelectedEnvironment.PythonVersion}";
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