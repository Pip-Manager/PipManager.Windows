using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using PipManager.Languages;
using PipManager.Services.Configuration;
using PipManager.Views.Pages.About;
using PipManager.Views.Pages.Library;
using PipManager.Views.Pages.Search;
using PipManager.Views.Pages.Settings;
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
            Content = Lang.MainWindowNavigationContentLibrary,
            Icon = new SymbolIcon { Symbol = SymbolRegular.Library24 },
            TargetPageType = typeof(LibraryPage)
        },
        new NavigationViewItem
        {
            Content = Lang.MainWindowNavigationContentSearch,
            Icon = new SymbolIcon { Symbol = SymbolRegular.BoxSearch24 },
            TargetPageType = typeof(SearchPage)
        }
    };

    [ObservableProperty]
    private ObservableCollection<object> _footerMenuItems = new()
    {
        new NavigationViewItem
        {
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = Lang.MainWindowNavigationContentSettings,
            Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
            TargetPageType = typeof(SettingsPage)
        },
        new NavigationViewItem
        {
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = Lang.MainWindowNavigationContentAbout,
            Icon = new SymbolIcon { Symbol = SymbolRegular.Info24 },
            TargetPageType = typeof(AboutPage)
        }
    };

    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems = new()
    {
        new MenuItem { Header = "Home", Tag = "tray_home" }
    };
}