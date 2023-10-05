using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Documents;
using System.Windows.Threading;
using PipManager.Models.AppConfigModels;
using PipManager.Models.Pages;
using PipManager.Models.PipInspection;
using PipManager.Services.Configuration;
using PipManager.Services.Environment;
using PipManager.ViewModels.Windows;
using PipManager.Views.Pages.Environment;
using Serilog;
using Wpf.Ui.Appearance;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IEnvironmentService _environmentService;
    private readonly IConfigurationService _configurationService;

    public LibraryViewModel(INavigationService navigationService, IEnvironmentService environmentService,
        IConfigurationService configurationService)
    {
        _navigationService = navigationService;
        _environmentService = environmentService;
        _configurationService = configurationService;

        Theme.Apply(_configurationService.AppConfig.Personalization.Theme switch
        {
            "light" => ThemeType.Light,
            "dark" => ThemeType.Dark,
            _ => ThemeType.Dark
        });
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _ = RefreshLibrary();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Library] Initialized");
    }

    [ObservableProperty] private int _libraryListLength;
    [ObservableProperty] private ObservableCollection<LibraryListItem> _libraryList = new();
    [ObservableProperty] private LibraryListItem? _selectedPackage;

    [ObservableProperty] private bool _loadingVisible;
    [ObservableProperty] private bool _environmentFoundVisible;
    [ObservableProperty] private bool _listVisible;

    [RelayCommand]
    private void NavigateToAddEnvironment()
    {
        _navigationService.Navigate(typeof(EnvironmentPage));
        _navigationService.Navigate(typeof(AddEnvironmentPage));
    }

    [RelayCommand]
    private async Task RefreshLibrary()
    {
        EnvironmentFoundVisible = true;
        LoadingVisible = true;
        ListVisible = false;
        if (_configurationService.AppConfig.CurrentEnvironment.PipDir == "")
        {
            LoadingVisible = false;
            EnvironmentFoundVisible = false;
            return;
        }
        var library = new List<PipMetadata>();
        await Task.Run(() =>
        {
            library = _environmentService.GetLibraries();
        }).ContinueWith(_ =>
        {
            LoadingVisible = false;
        });
        if (library != null)
        {
            LibraryList = new ObservableCollection<LibraryListItem>();
            foreach (var package in library)
            {
                LibraryList.Add(new LibraryListItem
                (
                    new SymbolIcon(SymbolRegular.Box24), package.Information.Name, package.Information.Version, package.Information.Summary
                ));
            }
            LibraryListLength = library.Count;
            ListVisible = true;
        }
    }
}