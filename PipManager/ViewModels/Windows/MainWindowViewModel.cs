using PipManager.Languages;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.Views.Pages.About;
using PipManager.Views.Pages.Environment;
using PipManager.Views.Pages.Library;
using PipManager.Views.Pages.Search;
using PipManager.Views.Pages.Settings;
using PipManager.Views.Pages.Tools;
using Serilog;
using System.Collections.ObjectModel;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IConfigurationService _configurationService;
    private readonly IEnvironmentService _environmentService;

    public MainWindowViewModel(IConfigurationService configurationService, IEnvironmentService environmentService)
    {
        _configurationService = configurationService;
        _environmentService = environmentService;
        if (_configurationService.AppConfig.CurrentEnvironment != null)
        {
            Log.Information($"[MainWindow] Environment loaded ({_configurationService.AppConfig.CurrentEnvironment.PipVersion} for {_configurationService.AppConfig.CurrentEnvironment.PythonVersion})");
            ApplicationTitle = $"Pip Manager | {_configurationService.AppConfig.CurrentEnvironment.PipVersion} for {_configurationService.AppConfig.CurrentEnvironment.PythonVersion}";
        }
        else
        {
            Log.Information("[MainWindow] No previous selected environment found");
        }
    }

    [ObservableProperty]
    private string _applicationTitle = "Pip Manager";

    [ObservableProperty]
    private ObservableCollection<object> _menuItems = new()
    {
        new NavigationViewItem
        {
            Content = Lang.MainWindow_NavigationContent_Library,
            Icon = new SymbolIcon { Symbol = SymbolRegular.Library24 },
            TargetPageType = typeof(LibraryPage)
        },
        new NavigationViewItem
        {
            Content = Lang.MainWindow_NavigationContent_Search,
            Icon = new SymbolIcon { Symbol = SymbolRegular.BoxSearch24 },
            TargetPageType = typeof(SearchPage)
        },
        new NavigationViewItem
        {
            Content = Lang.MainWindow_NavigationContent_Tools,
            Icon = new SymbolIcon { Symbol = SymbolRegular.Toolbox24 },
            TargetPageType = typeof(ToolsPage)
        }
    };

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = new()
    {
        new NavigationViewItem
        {
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = Lang.MainWindow_NavigationContent_Environment,
            Icon = new SymbolIcon { Symbol = SymbolRegular.AlignSpaceEvenlyHorizontal20 },
            TargetPageType = typeof(EnvironmentPage)
        },
        new NavigationViewItem
        {
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = Lang.MainWindow_NavigationContent_Settings,
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(SettingsPage)
        },
        new NavigationViewItem
        {
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = Lang.MainWindow_NavigationContent_About,
            Icon = new SymbolIcon { Symbol = SymbolRegular.Info24 },
            TargetPageType = typeof(AboutPage)
        }
    };
}