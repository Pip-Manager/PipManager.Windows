using PipManager.Services.Configuration;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly ISnackbarService _snackbarService;
    private readonly IConfigurationService _configurationService;
    public SettingsViewModel(ISnackbarService snackbarService, IConfigurationService configurationService)
    {
        _snackbarService = snackbarService;
        _configurationService = configurationService;
    }
    private bool _isInitialized;

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        CurrentTheme = _configurationService.AppConfig.Personalization.Theme switch
        {
            "light" => ThemeType.Light,
            "dark" => ThemeType.Dark,
            _ => ThemeType.Dark
        };
        LogAutoDeletion = _configurationService.AppConfig.Personalization.LogAutoDeletion;
        LogAutoDeletionTimes = _configurationService.AppConfig.Personalization.LogAutoDeletionTimes;
        CrushesAutoDeletion = _configurationService.AppConfig.Personalization.CrushesAutoDeletion;
        CrushesAutoDeletionTimes = _configurationService.AppConfig.Personalization.CrushesAutoDeletionTimes;

        _isInitialized = true;
    }

    #region Personalization
    
    [ObservableProperty] private ThemeType _currentTheme = ThemeType.Unknown;

    [RelayCommand]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "light":
                Theme.Apply(ThemeType.Light);
                CurrentTheme = ThemeType.Light;
                break;

            default:
                Theme.Apply(ThemeType.Dark);
                CurrentTheme = ThemeType.Dark;
                break;
        }
        _configurationService.AppConfig.Personalization.Theme = parameter;
        _configurationService.Save();
    }

    #endregion

    #region Log and Crushes Auto Deletion
    
    [ObservableProperty] private bool _logAutoDeletion = false;
    [ObservableProperty] private bool _crushesAutoDeletion = false;
    [ObservableProperty] private int _logAutoDeletionTimes = 0;
    [ObservableProperty] private int _crushesAutoDeletionTimes = 0;
    
    [RelayCommand]
    private void OnChangeLogAutoDeletion()
    {
        _configurationService.AppConfig.Personalization.LogAutoDeletion = LogAutoDeletion;
        _configurationService.Save();
    }
    
    [RelayCommand]
    private void OnChangeLogAutoDeletionTimes()
    {
        _configurationService.AppConfig.Personalization.LogAutoDeletionTimes = LogAutoDeletionTimes;
        _configurationService.Save();
    }
    
    [RelayCommand]
    private void OnChangeCrushesAutoDeletion()
    {
        _configurationService.AppConfig.Personalization.CrushesAutoDeletion = CrushesAutoDeletion;
        _configurationService.Save();
    }
    
    [RelayCommand]
    private void OnChangeCrushesAutoDeletionTimes()
    {
        _configurationService.AppConfig.Personalization.CrushesAutoDeletionTimes = CrushesAutoDeletionTimes;
        _configurationService.Save();
    }

    #endregion
}