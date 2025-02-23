﻿using PipManager.Core.Configuration;
using Serilog;

namespace PipManager.Windows.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty] private bool _debugMode = App.IsDebugMode;
    [ObservableProperty] private bool _isTitleBarCoverageGridVisible;
    [ObservableProperty] private string _applicationTitle = "Pip Manager";

    public MainWindowViewModel()
    {
        var config = Configuration.AppConfig;
        if (config.SelectedEnvironment != null)
        {
            Log.Information($"[MainWindow] Environment loaded ({config.SelectedEnvironment.PipVersion} for {config.SelectedEnvironment.PythonVersion})");
            ApplicationTitle = $"Pip Manager | {config.SelectedEnvironment.PipVersion} for {config.SelectedEnvironment.PythonVersion}";
        }
        else
        {
            Log.Information("[MainWindow] No previous selected environment found");
            ApplicationTitle = "Pip Manager";
        }
    }
}