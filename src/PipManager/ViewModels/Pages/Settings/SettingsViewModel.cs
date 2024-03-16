using Newtonsoft.Json;
using PipManager.Languages;
using PipManager.Models;
using PipManager.Models.Package;
using PipManager.Services.Configuration;
using PipManager.Views.Pages.About;
using PipManager.Views.Pages.Settings;
using Serilog;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace PipManager.ViewModels.Pages.Settings;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly HttpClient _httpClient;
    private readonly ISnackbarService _snackbarService;
    private readonly IConfigurationService _configurationService;
    private readonly IThemeService _themeService;
    private readonly INavigationService _navigationService;

    public SettingsViewModel(ISnackbarService snackbarService, IConfigurationService configurationService, IThemeService themeService, INavigationService navigationService)
    {
        _httpClient = App.GetService<HttpClient>();
        _snackbarService = snackbarService;
        _configurationService = configurationService;
        _themeService = themeService;
        _navigationService = navigationService;

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
            "light" => ApplicationTheme.Light,
            "dark" => ApplicationTheme.Dark,
            _ => ApplicationTheme.Dark
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

        async Task OfficialTask()
        {
            try
            {
                await _httpClient.GetByteArrayAsync(_configurationService.GetTestingUrlFromPackageSourceType(PackageSourceType.Official));
                OfficialPackageSourceNetwork = $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                OfficialPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTestFailed;
            }
        }

        async Task TsinghuaTask()
        {
            try
            {
                await _httpClient.GetByteArrayAsync(_configurationService.GetTestingUrlFromPackageSourceType(PackageSourceType.Tsinghua));
                TsinghuaPackageSourceNetwork = $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                TsinghuaPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTestFailed;
            }
        }

        async Task AliyunTask()
        {
            try
            {
                await _httpClient.GetByteArrayAsync(_configurationService.GetTestingUrlFromPackageSourceType(PackageSourceType.Aliyun));
                AliyunPackageSourceNetwork = $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                AliyunPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTestFailed;
            }
        }

        async Task DoubanTask()
        {
            try
            {
                await _httpClient.GetByteArrayAsync(_configurationService.GetTestingUrlFromPackageSourceType(PackageSourceType.Douban));
                DoubanPackageSourceNetwork = $"{stopwatch.ElapsedMilliseconds} ms";
            }
            catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
            {
                DoubanPackageSourceNetwork = Lang.Settings_PackageSource_NetworkTestFailed;
            }
        }

        await Task.WhenAll(OfficialTask(), TsinghuaTask(), AliyunTask(), DoubanTask());
        Log.Information($"[Settings] Package Source network tested: Official({OfficialPackageSourceNetwork}) Tsinghua({TsinghuaPackageSourceNetwork}) Aliyun({AliyunPackageSourceNetwork}) Douban({DoubanPackageSourceNetwork})");
    }

    #endregion Package Source

    #region Language

    [ObservableProperty] private List<string> _languages = ["Auto"];
    [ObservableProperty] private string _language = "Auto";

    [RelayCommand]
    private void OnChangeLanguage()
    {
        var language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        I18NExtension.Culture = language != "Auto" ? new CultureInfo(language) : CultureInfo.CurrentCulture;
        _configurationService.AppConfig.Personalization.Language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        _configurationService.Save();
        if (_isInitialized)
        {
            _navigationService.Navigate(typeof(AboutPage));
            _navigationService.Navigate(typeof(SettingsPage));
        }
        Log.Information($"[Settings] Language changes to {Language}");
    }

    #endregion Language

    #region Theme

    [ObservableProperty] private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

    [RelayCommand]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "light":
                _themeService.SetTheme(ApplicationTheme.Light);
                CurrentTheme = ApplicationTheme.Light;
                break;

            default:
                _themeService.SetTheme(ApplicationTheme.Dark);
                CurrentTheme = ApplicationTheme.Dark;
                break;
        }
        _configurationService.AppConfig.Personalization.Theme = parameter;
        _configurationService.Save();
        Log.Information($"[Settings] Theme changes to {parameter}");
    }

    #endregion Theme

    #region Log and Crushes Auto Deletion

    [ObservableProperty] private bool _logAutoDeletion;
    [ObservableProperty] private bool _crushesAutoDeletion;
    [ObservableProperty] private int _logAutoDeletionTimes;
    [ObservableProperty] private int _crushesAutoDeletionTimes;

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
            Title = Lang.ContentDialog_Title_AreYouSure,
            Content = Lang.Settings_FileManagement_ResetConfig_DialogContent,
            PrimaryButtonText = Lang.ContentDialog_PrimaryButton_Action,
            CloseButtonText = Lang.ContentDialog_CloseButton_Cancel
        };

        var result = await messageBox.ShowDialogAsync();
        if (result == Wpf.Ui.Controls.MessageBoxResult.Primary)
        {
            Log.Information("Config reset");
            await File.WriteAllTextAsync(AppInfo.ConfigPath, JsonConvert.SerializeObject(new AppConfig(), Formatting.Indented));
        }
    }

    #endregion File Management
}