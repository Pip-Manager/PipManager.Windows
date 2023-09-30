using PipManager.Languages;
using PipManager.Services.Configuration;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Settings;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly ISnackbarService _snackbarService;
    private readonly IConfigurationService _configurationService;
    public SettingsViewModel(ISnackbarService snackbarService, IConfigurationService configurationService)
    {
        _snackbarService = snackbarService;
        _configurationService = configurationService;

        foreach (var languagePair in GetLanguage.LanguageList) 
        {
            Languages.Add(languagePair.Key);
        }
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
        var language = _configurationService.AppConfig.Personalization.Language;
        Language = language != "Auto" ? GetLanguage.LanguageList.Select(x => x.Key).ToList()[GetLanguage.LanguageList.Select(x => x.Value).ToList().IndexOf(language)] : "Auto";
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

    #region Language

    [ObservableProperty] private List<string> _languages = new() {"Auto"};
    [ObservableProperty] private string _language = "Auto";

    [RelayCommand]
    private void OnChangeLanguage()
    {
        _configurationService.AppConfig.Personalization.Language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        _configurationService.Save();
    }

    #endregion

    #region Theme
    
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