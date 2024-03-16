using Microsoft.Web.WebView2.Wpf;
using Wpf.Ui.Controls;
using SearchDetailViewModel = PipManager.ViewModels.Pages.Search.SearchDetailViewModel;

namespace PipManager.Views.Pages.Search;

public partial class SearchDetailPage : INavigableView<SearchDetailViewModel>
{
    public static WebView2? ProjectDescriptionWebView { get; set; }

    public SearchDetailViewModel ViewModel { get; }

    public SearchDetailPage(SearchDetailViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
        ProjectDescriptionWebView = SearchDetailProjectDesciptionWebView;
    }

    private void SearchDetailProjectDesciptionWebView_NavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
    {
        if (e.Uri.StartsWith("http://") || e.Uri.StartsWith("https://"))
        {
            e.Cancel = true;
        }
    }

    private void SearchDetailProjectDesciptionWebView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
    {
        if (e != null && e.IsSuccess)
        {
            ProjectDescriptionWebView!.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
               "document.addEventListener('contextmenu', event => event.preventDefault());");
        }
    }
}