using Newtonsoft.Json;
using PipManager.Languages;
using PipManager.Models;
using PipManager.Services.Configuration;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult;

namespace PipManager.ViewModels.Pages.Settings;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly HttpClient _httpClient;
    private readonly ISnackbarService _snackbarService;
    private readonly IConfigurationService _configurationService;

    public SettingsViewModel(ISnackbarService snackbarService, IConfigurationService configurationService)
    {
        _httpClient = App.GetService<HttpClient>();
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
        CurrentPackageSource = _configurationService.AppConfig.PackageSource.PackageSourceType;
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
        Log.Information("[Settings] Initialized");
    }

    #region Package Source

    [ObservableProperty] private PackageSourceType _currentPackageSource = PackageSourceType.Official;
    [ObservableProperty] private string _officialPackageSourceNetwork = string.Empty;
    [ObservableProperty] private string _tsinghuaPackageSourceNetwork = string.Empty;
    [ObservableProperty] private string _aliyunPackageSourceNetwork = string.Empty;
    [ObservableProperty] private string _doubanPackageSourceNetwork = string.Empty;

    [RelayCommand]
    private void OnChangePackageSource(string parameter)
    {
        CurrentPackageSource = parameter switch
        {
            "official" => PackageSourceType.Official,
            "tsinghua" => PackageSourceType.Tsinghua,
            "aliyun" => PackageSourceType.Aliyun,
            "douban" => PackageSourceType.Douban,
            _ => PackageSourceType.Official
        };
        _configurationService.AppConfig.PackageSource.PackageSourceType = CurrentPackageSource;
        _configurationService.Save();
        Log.Information($"[Settings] Package source changes to {parameter}");
    }

    [RelayCommand]
    private async Task OnTestNetwork()
    {
        OfficialPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTesting;
        TsinghuaPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTesting;
        AliyunPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTesting;
        DoubanPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTesting;
        var stopwatch = Stopwatch.StartNew();
        var officialTask = async () =>
        {
            try
            {
                await _httpClient.GetByteArrayAsync(_configurationService.GetTestingUrlFromPackageSourceType(PackageSourceType.Official));
                OfficialPackageSourceNetwork = $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch (HttpRequestException)
            {
                OfficialPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTestFailed;
            }
        };
        var tsinghuaTask = async () =>
        {
            try
            {
                await _httpClient.GetByteArrayAsync(_configurationService.GetTestingUrlFromPackageSourceType(PackageSourceType.Tsinghua));
                TsinghuaPackageSourceNetwork = $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch (HttpRequestException)
            {
                TsinghuaPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTestFailed;
            }
        };
        var aliyunTask = async () =>
        {
            try
            {
                await _httpClient.GetByteArrayAsync(_configurationService.GetTestingUrlFromPackageSourceType(PackageSourceType.Aliyun));
                AliyunPackageSourceNetwork = $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch (HttpRequestException)
            {
                AliyunPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTestFailed;
            }
        };
        var doubanTask = async () =>
        {
            try
            {
                await _httpClient.GetByteArrayAsync(_configurationService.GetTestingUrlFromPackageSourceType(PackageSourceType.Douban));
                DoubanPackageSourceNetwork = $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch (HttpRequestException)
            {
                DoubanPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTestFailed;
            }
        };
        await Task.WhenAll(officialTask(), tsinghuaTask(), aliyunTask(), doubanTask());
        Log.Information($"[Settings] Package Source network tested: Official({OfficialPackageSourceNetwork}) Tsinghua({TsinghuaPackageSourceNetwork}) Aliyun({AliyunPackageSourceNetwork}) Douban({DoubanPackageSourceNetwork})");
    }

    #endregion Package Source

    #region Language

    [ObservableProperty] private List<string> _languages = new() { "Auto" };
    [ObservableProperty] private string _language = "Auto";

    [RelayCommand]
    private void OnChangeLanguage()
    {
        if (_isInitialized)
        {
            _snackbarService.Show(Lang.Common_NoticeTitle_Caution, Lang.Snackbar_effectAfterRestart);
        }
        _configurationService.AppConfig.Personalization.Language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        _configurationService.Save();
        Log.Information($"[Settings] Language changes to {Language}");
    }

    #endregion Language

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

    #endregion Theme

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

    #endregion Log and Crushes Auto Deletion

    #region File Management

    [RelayCommand]
    private static void OpenAppFolder()
    {
        Process.Start("explorer.exe", System.Environment.CurrentDirectory);
        Log.Information("[Settings] App folder opened");
    }

    [RelayCommand]
    private static void OpenLogFolder()
    {
        if (!Directory.Exists(AppInfo.LogDir)) return;
        Process.Start("explorer.exe", AppInfo.LogDir);
        Log.Information("[Settings] Log folder opened");
    }

    [RelayCommand]
    private void OpenCrushesFolder()
    {
        if (Directory.Exists(AppInfo.CrushesDir))
        {
            Process.Start("explorer.exe", AppInfo.CrushesDir);
        }
        else
        {
            _snackbarService.Show(Lang.Common_NoticeTitle_Caution, Lang.Settings_FileManagement_CrushesDirNotFound);
            Log.Information("[Settings] Crushes folder not found");
        }
    }

    [RelayCommand]
    private static async Task ResetConfigurationAsync()
    {
        var messageBox = new Wpf.Ui.Controls.MessageBox
        {
            Title = Lang.MessageDialog_Title_AreYouSure,
            Content = Lang.Settings_FileManagement_ResetConfig_DialogContent,
            PrimaryButtonText = Lang.MessageDialog_PrimaryButton_Action,
            CloseButtonText = Lang.MessageDialog_CloseButton_Cancel
        };

        var result = await messageBox.ShowDialogAsync();
        if (result == MessageBoxResult.Primary)
        {
            Log.Information("Config reset");
            await File.WriteAllTextAsync(AppInfo.ConfigPath, JsonConvert.SerializeObject(new AppConfig(), Formatting.Indented));
        }
    }

    #endregion File Management
}