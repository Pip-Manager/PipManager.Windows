﻿using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Wpf.Ui.Controls;
using Search_SearchDetailViewModel = PipManager.Windows.ViewModels.Pages.Search.SearchDetailViewModel;
using SearchDetailViewModel = PipManager.Windows.ViewModels.Pages.Search.SearchDetailViewModel;

namespace PipManager.Windows.Views.Pages.Search;

public partial class SearchDetailPage : INavigableView<Search_SearchDetailViewModel>
{
    public static WebView2? ProjectDescriptionWebView { get; private set; }

    public Search_SearchDetailViewModel ViewModel { get; }

    public SearchDetailPage(Search_SearchDetailViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
        ProjectDescriptionWebView = SearchDetailProjectDescriptionWebView;
    }

    private void SearchDetailProjectDescriptionWebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
    {
        if (e.Uri.StartsWith("http://") || e.Uri.StartsWith("https://"))
        {
            e.Cancel = true;
        }
    }

    private void SearchDetailProjectDescriptionWebView_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs? e)
    {
        if (e is { IsSuccess: true })
        {
            ProjectDescriptionWebView!.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(
               "document.addEventListener('contextmenu', event => event.preventDefault());");
        }
    }
}