using PipManager.Models.AppConfigModels;
using PipManager.Services.Configuration;
using PipManager.Views.Pages.Environment;
using Serilog;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Environment;

public partial class EnvironmentViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly IConfigurationService _configurationService;

    public EnvironmentViewModel(INavigationService navigationService, IConfigurationService configurationService)
    {
        _navigationService = navigationService;
        _configurationService = configurationService;
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        EnvironmentItems = new ObservableCollection<EnvironmentItem>(_configurationService.AppConfig.EnvironmentItems);
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Environment] Initialized");
    }

    #region Add Environment

    [ObservableProperty]
    private ObservableCollection<EnvironmentItem> _environmentItems = new();

    [RelayCommand]
    private void AddEnvironment()
    {
        _navigationService.Navigate(typeof(AddEnvironmentPage));
    }

    #endregion Add Environment
}