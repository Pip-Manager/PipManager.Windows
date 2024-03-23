using CommunityToolkit.Mvvm.Messaging;
using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using PipManager.Languages;
using PipManager.Models.Pages;
using PipManager.PackageSearch.Wrappers.Query;
using PipManager.Services.Environment;
using PipManager.Services.Mask;
using PipManager.Services.Toast;
using PipManager.Views.Pages.Search;
using Serilog;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Net.Http;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Search;

public partial class SearchDetailViewModel : ObservableObject, INavigationAware
{
    public record SearchDetailMessage(QueryListItemModel Package);
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly HttpClient _httpClient;
    private readonly IThemeService _themeService;
    private readonly IToastService _toastService;
    private readonly IMaskService _maskService;
    private readonly IEnvironmentService _environmentService;

    [ObservableProperty]
    private bool _projectDescriptionVisibility = false;

    private string _themeType = "light";

    [ObservableProperty]
    private string _themeTypeInHex = "#FAFAFA";

    private int _themeTypeInInteger = 16448250;

    private const string _htmlModel = """
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

    public SearchDetailViewModel(INavigationService navigationService, HttpClient httpClient, IThemeService themeService, IToastService toastService, IMaskService maskService, IEnvironmentService environmentService)
    {
        _navigationService = navigationService;
        _httpClient = httpClient;
        _themeService = themeService;
        _toastService = toastService;
        _maskService = maskService;
        _environmentService = environmentService;

        WeakReferenceMessenger.Default.Register<SearchDetailMessage>(this, Receive);
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Collapsed;
        switch (_themeService.GetTheme())
        {
            case Wpf.Ui.Appearance.ApplicationTheme.Light:
                _themeType = "light";
                ThemeTypeInHex = "#FFFFFF";
                _themeTypeInInteger = 16777215;
                break;

            case Wpf.Ui.Appearance.ApplicationTheme.Dark:
                _themeType = "dark";
                ThemeTypeInHex = "#0D1117";
                _themeTypeInInteger = 856343;
                break;
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
    private async Task InstallPackage()
    {
        var installedPackages = await _environmentService.GetLibraries();
        if(installedPackages == null)
        {
            _toastService.Error(Lang.SearchDetail_Install_CannotGetLibraries);
            return;
        }
        if (installedPackages.Any(item => item.Name == Package!.Name))
        {
            _toastService.Error(Lang.LibraryInstall_Add_AlreadyInstalled);
            return;
        }
        
    }

    public void Receive(object recipient, SearchDetailMessage message)
    {
        Package = message.Package;

        SearchDetailPage.ProjectDescriptionWebView!.Loaded += async (sender, e) =>
        {
            ProjectDescriptionVisibility = false;
            var packageVersions = await _environmentService.GetVersions(Package!.Name);
            switch (packageVersions.Status)
            {
                case 1:
                    _toastService.Error(Lang.LibraryInstall_Add_PackageNotFound);
                    break;

                case 2:
                    _toastService.Error(Lang.LibraryInstall_Add_InvalidPackageName);
                    break;

                default:
                    AvailableVersions = new ObservableCollection<string>(packageVersions.Versions!.Reverse());
                    TargetVersion = AvailableVersions.First();
                    break;
            }
            var webView2Environment = await CoreWebView2Environment.CreateAsync(null, AppInfo.CachesDir);
            await SearchDetailPage.ProjectDescriptionWebView!.EnsureCoreWebView2Async().ConfigureAwait(true);
            try
            {
                var projectDescriptionUrl = message.Package.Url;
                var html = await _httpClient.GetStringAsync(projectDescriptionUrl);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                string projectDescriptionHtml = string.Format(_htmlModel, _themeType, ThemeTypeInHex, htmlDocument.DocumentNode.SelectSingleNode("//*[@id=\"description\"]/div").InnerHtml);

                SearchDetailPage.ProjectDescriptionWebView!.CoreWebView2.Profile.PreferredColorScheme = CoreWebView2PreferredColorScheme.Dark;
                SearchDetailPage.ProjectDescriptionWebView!.NavigateToString(projectDescriptionHtml);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                _toastService.Error(Lang.SearchDetail_ProjectDescription_LoadFailed);
                string projectDescriptionHtml = string.Format(_htmlModel, _themeType, ThemeTypeInHex, $"<p>{Lang.SearchDetail_ProjectDescription_LoadFailed}</p>");

                SearchDetailPage.ProjectDescriptionWebView!.CoreWebView2.Profile.PreferredColorScheme = CoreWebView2PreferredColorScheme.Dark;
                SearchDetailPage.ProjectDescriptionWebView!.NavigateToString(projectDescriptionHtml);
            }
            finally
            {
                await Task.Delay(500);
                ProjectDescriptionVisibility = true;
            }
        };
    }
}