using CommunityToolkit.Mvvm.Messaging;
using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using Serilog;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net.Http;
using Microsoft.Win32;
using PipManager.Core.Wrappers.PackageSearchQueryWrapper;
using PipManager.Windows.Languages;
using PipManager.Windows.Models.Action;
using PipManager.Windows.Services.Action;
using PipManager.Windows.Services.Configuration;
using PipManager.Windows.Services.Environment;
using PipManager.Windows.Services.Toast;
using PipManager.Windows.Views.Pages.Search;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PipManager.Windows.ViewModels.Pages.Search;

public partial class SearchDetailViewModel : ObservableObject, INavigationAware
{
    public record SearchDetailMessage(QueryListItemModel Package);
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly HttpClient _httpClient;
    private readonly IThemeService _themeService;
    private readonly IToastService _toastService;
    private readonly IActionService _actionService;
    private readonly IEnvironmentService _environmentService;
    private readonly IConfigurationService _configurationService;

    [ObservableProperty]
    private bool _projectDescriptionVisibility;

    private string _themeType = "light";

    [ObservableProperty]
    private string _themeTypeInHex = "#FAFAFA";

    private int _themeTypeInInteger = 16448250;

    private const string HtmlModel = """
        <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8" />
                <meta name="color-scheme" content="{0}">
                <link href="https://cdnjs.cloudflare.com/ajax/libs/github-markdown-css/5.2.0/github-markdown-{0}.min.css" type="text/css" rel="stylesheet" />
            </head>
            <body style="background-color:{1};">
                <article class="markdown-body">
                    {2}
                </article>
            </body>
        </html>
        """;

    [ObservableProperty]
    private QueryListItemModel? _package;

    public SearchDetailViewModel(INavigationService navigationService, HttpClient httpClient, IThemeService themeService, IToastService toastService, IEnvironmentService environmentService, IActionService actionService, IConfigurationService configurationService)
    {
        _navigationService = navigationService;
        _httpClient = httpClient;
        _themeService = themeService;
        _toastService = toastService;
        _environmentService = environmentService;
        _actionService = actionService;
        _configurationService = configurationService;

        WeakReferenceMessenger.Default.Register<SearchDetailMessage>(this, Receive);
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Collapsed;
        switch (_themeService.GetTheme())
        {
            case ApplicationTheme.Light:
                _themeType = "light";
                ThemeTypeInHex = "#FFFFFF";
                _themeTypeInInteger = 16777215;
                break;

            case ApplicationTheme.Dark:
                _themeType = "dark";
                ThemeTypeInHex = "#0D1117";
                _themeTypeInInteger = 856343;
                break;
            case ApplicationTheme.Unknown:
            case ApplicationTheme.HighContrast:
            default:
                throw new ArgumentOutOfRangeException();
        }
        SearchDetailPage.ProjectDescriptionWebView!.DefaultBackgroundColor = Color.FromArgb(_themeTypeInInteger);
    }

    public void OnNavigatedFrom()
    {
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Visible;
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
    }

    [ObservableProperty]
    private ObservableCollection<string> _availableVersions = [];

    [ObservableProperty]
    private string _targetVersion = "";

    [RelayCommand]
    private void DownloadPackage()
    {
        var openFolderDialog = new OpenFolderDialog
        {
            Title = Lang.Dialog_Title_DownloadDistributions
        };
        var result = openFolderDialog.ShowDialog();
        if (result != true)
        {
            return;
        }
        _actionService.AddOperation(new ActionListItem
        (
            ActionType.Download,
            [$"{Package!.Name}=={TargetVersion}"],
            path: openFolderDialog.FolderName,
            extraParameters: ["--no-deps"]
        ));
    }

    [RelayCommand]
    private async Task InstallPackage()
    {
        var installedPackages = await _environmentService.GetLibraries();
        if (installedPackages == null)
        {
            _toastService.Error(Lang.SearchDetail_Install_CannotGetLibraries);
            return;
        }
        if (installedPackages.Any(item => item.Name == Package!.Name))
        {
            _toastService.Error(Lang.LibraryInstall_Add_AlreadyInstalled);
            return;
        }
        _actionService.AddOperation(new ActionListItem
        (
            ActionType.Install,
            [$"{Package!.Name}=={TargetVersion}"]
        ));
    }

    private void Receive(object recipient, SearchDetailMessage message)
    {
        Package = message.Package;

        SearchDetailPage.ProjectDescriptionWebView!.Loaded += async (_, _) =>
        {
            ProjectDescriptionVisibility = false;
            var packageVersions = await _environmentService.GetVersions(Package!.Name, new CancellationToken(), _configurationService.AppConfig.PackageSource.AllowNonRelease);
            switch (packageVersions.Status)
            {
                case 1 or 2:
                    _toastService.Error(Lang.SearchDetail_Exception_NetworkError);
                    await Task.Delay(1000);
                    _navigationService.GoBack();
                    return;

                default:
                    AvailableVersions = new ObservableCollection<string>(packageVersions.Versions!.Reverse());
                    TargetVersion = AvailableVersions.First();
                    break;
            }
            await CoreWebView2Environment.CreateAsync(null, AppInfo.CachesDir);
            await SearchDetailPage.ProjectDescriptionWebView.EnsureCoreWebView2Async().ConfigureAwait(true);
            try
            {
                var projectDescriptionUrl = message.Package.Url;
                var html = await _httpClient.GetStringAsync(projectDescriptionUrl);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                string projectDescriptionHtml = string.Format(HtmlModel, _themeType, ThemeTypeInHex, htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"description\"]/div").InnerHtml);

                SearchDetailPage.ProjectDescriptionWebView.CoreWebView2.Profile.PreferredColorScheme = CoreWebView2PreferredColorScheme.Dark;
                SearchDetailPage.ProjectDescriptionWebView.NavigateToString(projectDescriptionHtml);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                _toastService.Error(Lang.SearchDetail_ProjectDescription_LoadFailed);
                string projectDescriptionHtml = string.Format(HtmlModel, _themeType, ThemeTypeInHex, $"<p>{Lang.SearchDetail_ProjectDescription_LoadFailed}</p>");

                SearchDetailPage.ProjectDescriptionWebView.CoreWebView2.Profile.PreferredColorScheme = CoreWebView2PreferredColorScheme.Dark;
                SearchDetailPage.ProjectDescriptionWebView.NavigateToString(projectDescriptionHtml);
            }
            finally
            {
                await Task.Delay(500);
                ProjectDescriptionVisibility = true;
            }
        };
    }
}