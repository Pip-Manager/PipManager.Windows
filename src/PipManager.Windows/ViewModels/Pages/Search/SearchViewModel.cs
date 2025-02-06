using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using PipManager.Core.Configuration;
using PipManager.Core.Extensions;
using PipManager.Core.PyPackage.Models;
using PipManager.Core.Wrappers.PackageSearchIndexWrapper;
using PipManager.Windows.Services.Mask;
using PipManager.Windows.Views.Pages.Search;
using Wpf.Ui;
using Wpf.Ui.Abstractions.Controls;
using Process = Raffinert.FuzzySharp.Process;

namespace PipManager.Windows.ViewModels.Pages.Search;

public partial class SearchViewModel(IMaskService maskService, INavigationService navigationService)
    : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty] private Dictionary<string, string>? _queryMapping;
    [ObservableProperty] private List<string>? _queryNameList = [];
    [ObservableProperty] private ObservableCollection<string> _searchResults = [];
    
    [ObservableProperty] private string _queryPackageName = "";
    [ObservableProperty] private string _totalResultNumber = "";
    [ObservableProperty] private bool _onQuerying;
    [ObservableProperty] private bool _successQueried;
    [ObservableProperty] private bool _reachesFirstPage = true;
    [ObservableProperty] private bool _reachesLastPage;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _maxPage = 1;

    private async Task<Dictionary<string, string>?> TryLoadIndex()
    {
        var packageSource = Configuration.AppConfig.PackageSource.Source switch
        {
            "official" => PackageSourceType.Official,
            "tsinghua" => PackageSourceType.Tsinghua,
            "aliyun" => PackageSourceType.Aliyun,
            "douban" => PackageSourceType.Douban,
            _ => throw new ArgumentOutOfRangeException()
        };
        var targetIndexFilePath = Path.Combine(AppInfo.CachesDir, $"{packageSource}-index.json");
        var indexContent =
            JsonSerializer.Deserialize<List<IndexItemModel>>(await File.ReadAllTextAsync(targetIndexFilePath));
        if (indexContent != null)
        {
            return File.Exists(targetIndexFilePath)
                ? indexContent.ToDictionary(key => key.Name, value => value.Url)
                : null;
        }

        return null;
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Search] Initialized");
    }

    #region Details
    
    [RelayCommand]
    private void SearchItemDoubleClick(string selectedPackageName)
    {
        var selectedIndexItem = new IndexItemModel
        {
            Name = selectedPackageName,
            Url = $"{Configuration.AppConfig.PackageSource.Source.GetPackageSourceUrl()}{QueryMapping![selectedPackageName]}"
        };
        navigationService.Navigate(typeof(SearchDetailPage));
        WeakReferenceMessenger.Default.Send(new SearchDetailViewModel.SearchDetailMessage(selectedIndexItem));
        Log.Information($"[Search] Turn to detail page: {selectedIndexItem.Name}");
    }

    #endregion Details

    [RelayCommand]
    private async Task Search(string? parameter)
    {
        if (parameter == null || QueryNameList == null)
        {
            return;
        }
        
        maskService.Show();
        SearchResults.Clear();
        await Task.Run(() =>
        {
            var searchText = parameter.ToLower();
            var fuzzyResults = Process.ExtractTop(searchText, QueryNameList, limit:1000);
            foreach (var fuzzyResult in fuzzyResults)
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    SearchResults.Add(fuzzyResult.Value);
                });
            }
        });
        Task.WaitAll();
        maskService.Hide();
    }

    public async Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        Application.Current.Dispatcher.InvokeAsync(() =>
        {
            navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Visible;
        });
        QueryMapping = await TryLoadIndex();
        QueryNameList = QueryMapping?.Keys.ToList();
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}