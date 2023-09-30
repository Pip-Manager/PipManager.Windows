using PipManager.Languages;
using PipManager.Services.Configuration;
using Serilog;
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
        Log.Information("[Settings] Language list items added");
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
        Log.Information("[Settings] Initialized");
        _isInitialized = true;
    }

    #region Language

    [ObservableProperty] private List<string> _languages = new() {"Auto"};
    [ObservableProperty] private string _language = "Auto";

    [RelayCommand]
    private void OnChangeLanguage()
    {
        if (_isInitialized)
        {
            _snackbarService.Show(Lang.Snackbar_Caution, Lang.Snackbar_effectAfterRestart, ControlAppearance.Caution);
        }
        _configurationService.AppConfig.Personalization.Language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        _configurationService.Save();
        Log.Information($"[Settings] Language changes to {Language}");
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
        Log.Information($"[Settings] Theme changes to {parameter}");
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
        Log.Information($"[Settings] Log auto deletion now is {LogAutoDeletion}");
    }

    [RelayCommand]
    private void OnChangeLogAutoDeletionTimes()
    {
        _configurationService.AppConfig.Personalization.LogAutoDeletionTimes = LogAutoDeletionTimes;
        _configurationService.Save();
        Log.Information($"[Settings] Log auto deletion will be executed when the number of files reaches {LogAutoDeletionTimes}");
    }

    [RelayCommand]
    private void OnChangeCrushesAutoDeletion()
    {
        _configurationService.AppConfig.Personalization.CrushesAutoDeletion = CrushesAutoDeletion;
        _configurationService.Save();
        Log.Information($"[Settings] Crushes auto deletion now is {CrushesAutoDeletion}");
    }

    [RelayCommand]
    private void OnChangeCrushesAutoDeletionTimes()
    {
        _configurationService.AppConfig.Personalization.CrushesAutoDeletionTimes = CrushesAutoDeletionTimes;
        _configurationService.Save();
        Log.Information($"[Settings] Crushes auto deletion will be executed when the number of files reaches {CrushesAutoDeletionTimes}");
    }

    #endregion
}