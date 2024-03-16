using CommunityToolkit.Mvvm.Messaging;
using PipManager.PackageSearch.Wrappers.Query;
using PipManager.Views.Pages.Search;
using Serilog;
using System.Net.Http;
using Wpf.Ui;
using Wpf.Ui.Controls;
using HtmlAgilityPack;
using System.Drawing;
using Microsoft.Web.WebView2.Core;
using PipManager.Services.Toast;
using PipManager.Languages;

namespace PipManager.ViewModels.Pages.Search;

public partial class SearchDetailViewModel : ObservableObject, INavigationAware
{
    public record SearchDetailMessage(QueryListItemModel Package);
    private bool _isInitialized;
    private readonly INavigationService _navigationService;
    private readonly HttpClient _httpClient;
    private readonly IThemeService _themeService;
    private readonly IToastService _toastService;

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

    public SearchDetailViewModel(INavigationService navigationService, HttpClient httpClient, IThemeService themeService, IToastService toastService)
    {
        _navigationService = navigationService;
        _httpClient = httpClient;
        _themeService = themeService;
        _toastService = toastService;

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

    public void Receive(object recipient, SearchDetailMessage message)
    {
        Package = message.Package;

        SearchDetailPage.ProjectDescriptionWebView!.Loaded += async (sender, e) =>
        {
            ProjectDescriptionVisibility = false;
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