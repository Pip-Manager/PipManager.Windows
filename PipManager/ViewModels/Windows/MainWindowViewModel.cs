using System.Collections.ObjectModel;
using PipManager.Services.Configuration;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Windows;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IConfigurationService _configurationService;
    public MainWindowViewModel(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
        _configurationService.Initialize();

    }
    [ObservableProperty]
    private string _applicationTitle = "Pip Manager";

    [ObservableProperty]
    private ObservableCollection<object> _menuItems = new()
    {
        new NavigationViewItem
        {
            Content = "Library",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Library24 },
            TargetPageType = typeof(Views.Pages.LibraryPage)
        },
        new NavigationViewItem
        {
            Content = "Add",
            Icon = new SymbolIcon { Symbol = SymbolRegular.AddCircle24 },
            TargetPageType = typeof(Views.Pages.AddPage)
        },
        new NavigationViewItem
        {
            Content = "Update",
            Icon = new SymbolIcon { Symbol = SymbolRegular.WindowNew24 },
            TargetPageType = typeof(Views.Pages.UpdatePage)
        }
    };

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = new()
    {
        new NavigationViewItem
        {
            Content = "Settings",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(Views.Pages.SettingsPage)
        }
    };

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems = new()
    {
        new MenuItem { Header = "Home", Tag = "tray_home" }
    };
}