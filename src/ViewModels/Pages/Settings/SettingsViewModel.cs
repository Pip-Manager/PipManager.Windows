using Serilog;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using PipManager.Core.Configuration;
using PipManager.Core.Extensions;
using PipManager.Windows.Languages;
using PipManager.Windows.Services.Toast;
using PipManager.Windows.Views.Pages.About;
using PipManager.Windows.Views.Pages.Settings;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;

namespace PipManager.Windows.ViewModels.Pages.Settings;

public partial class SettingsViewModel : ObservableObject, INavigationAware
{
    private readonly HttpClient _httpClient;
    private readonly ISnackbarService _snackbarService;
    private readonly IThemeService _themeService;
    private readonly INavigationService _navigationService;
    private readonly IToastService _toastService;

    public SettingsViewModel(ISnackbarService snackbarService, IThemeService themeService, INavigationService navigationService, IToastService toastService)
    {
        _httpClient = App.GetService<HttpClient>();
        _snackbarService = snackbarService;
        _themeService = themeService;
        _navigationService = navigationService;
        _toastService = toastService;

        foreach (var languagePair in GetLanguage.LanguageList)
        {
            Languages.Add(languagePair.Key);
        }
        Log.Information("[Settings] Language list items added");
    }

    private bool _isInitialized;

    private void InitializeViewModel()
    {
        var config = Configuration.AppConfig!;
        CurrentPackageSource = config.PackageSource.Source;
        AllowNonRelease = config.PackageSource.AllowNonRelease;
        var language = config.Personalization.Language;
        Language = language != "Auto" ? GetLanguage.LanguageList.Select(x => x.Key).ToList()[GetLanguage.LanguageList.Select(x => x.Value).ToList().IndexOf(language)] : "Auto";
        CurrentTheme = config.Personalization.Theme switch
        {
            "light" => ApplicationTheme.Light,
            "dark" => ApplicationTheme.Dark,
            _ => ApplicationTheme.Dark
        };
        _isInitialized = true;
        Log.Information("[Settings] Initialized");
    }

    #region Package Source

    [ObservableProperty] private string _currentPackageSource = "default";
    [ObservableProperty] private string _officialPackageSourceNetwork = string.Empty;
    [ObservableProperty] private string _tsinghuaPackageSourceNetwork = string.Empty;
    [ObservableProperty] private string _aliyunPackageSourceNetwork = string.Empty;
    [ObservableProperty] private string _doubanPackageSourceNetwork = string.Empty;

    [RelayCommand]
    private void OnChangePackageSource(string parameter)
    {
        Configuration.AppConfig!.PackageSource.Source = parameter;
        Configuration.Save();
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
                await _httpClient.GetByteArrayAsync("default".GetPackageSourceTestingUrl());
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
                await _httpClient.GetByteArrayAsync("tsinghua".GetPackageSourceTestingUrl());
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
                await _httpClient.GetByteArrayAsync("aliyun".GetPackageSourceTestingUrl());
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
                await _httpClient.GetByteArrayAsync("douban".GetPackageSourceTestingUrl());
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
    
    [ObservableProperty]
    private bool _allowNonRelease;
    
    [RelayCommand]
    private void OnChangeDetectNonReleaseVersion()
    {
        Configuration.AppConfig!.PackageSource.AllowNonRelease = AllowNonRelease;
        Configuration.Save();
        Log.Information($"[Settings] Detect non-release update changes to {AllowNonRelease}");
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
        Configuration.AppConfig!.Personalization.Language = Language != "Auto" ? GetLanguage.LanguageList[Language] : "Auto";
        Configuration.Save();
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
        Configuration.AppConfig!.Personalization.Theme = parameter;
        Configuration.Save();
        Log.Information($"[Settings] Theme changes to {parameter}");
    }

    #endregion Theme

    #region File Management

    [RelayCommand]
    private void WebViewClearCache()
    {
        try
        {
            Directory.Delete(Path.Combine(AppInfo.CachesDir, "EBWebView"), true);
            _toastService.Success(Lang.Settings_FileManagement_WebViewSettings_CacheCleared);
            Log.Information($"[Settings] WebView cache removed");
        }
        catch (DirectoryNotFoundException)
        {
            _toastService.Success(Lang.Settings_FileManagement_WebViewSettings_CacheCleared);
        }
        catch (IOException)
        {
            _toastService.Error(Lang.Settings_FileManagement_WebViewSettings_CacheIsUsing);
        }
    }

    [ObservableProperty]
    private string _appFolderPath = System.Environment.CurrentDirectory;

    [RelayCommand]
    private void OpenAppFolder()
    {
        Process.Start("explorer.exe", AppFolderPath);
        Log.Information("[Settings] App folder opened");
    }

    [ObservableProperty]
    private string _logFolderPath = AppInfo.LogDir;

    [RelayCommand]
    private void OpenLogFolder()
    {
        if (!Directory.Exists(LogFolderPath)) return;
        Process.Start("explorer.exe", LogFolderPath);
        Log.Information("[Settings] Log folder opened");
    }

    [ObservableProperty]
    private string _crushesFolderPath = AppInfo.CrushesDir;

    [RelayCommand]
    private void OpenCrushesFolder()
    {
        if (Directory.Exists(CrushesFolderPath))
        {
            Process.Start("explorer.exe", CrushesFolderPath);
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
            Configuration.Reset();
        }
    }

    #endregion File Management

    public Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}