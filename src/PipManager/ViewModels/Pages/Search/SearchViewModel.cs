using CommunityToolkit.Mvvm.Messaging;
using PipManager.Languages;
using PipManager.PackageSearch;
using PipManager.PackageSearch.Wrappers.Query;
using PipManager.Services.Mask;
using PipManager.Services.Toast;
using PipManager.Views.Pages.Search;
using Serilog;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;
using static PipManager.ViewModels.Pages.Search.SearchDetailViewModel;

namespace PipManager.ViewModels.Pages.Search;

public partial class SearchViewModel(IPackageSearchService packageSearchService, IToastService toastService, IMaskService maskService, INavigationService navigationService) : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty]
    private ObservableCollection<QueryListItemModel> _queryList = [];

    [ObservableProperty]
    private string _queryPackageName = "";

    [ObservableProperty]
    private string _totalResultNumber = "";

    [ObservableProperty]
    private bool _onQuerying;

    [ObservableProperty]
    private bool _successQueried;

    [ObservableProperty]
    private bool _reachesFirstPage = true;

    [ObservableProperty]
    private bool _reachesLastPage;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _maxPage = 1;

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
        _isInitialized = true;
        Log.Information("[Search] Initialized");
    }

    #region Details

    [RelayCommand]
    private void ToDetailPage(object parameter)
    {
        navigationService.Navigate(typeof(SearchDetailPage));
        var current = QueryList.Where(searchListItem => searchListItem.Name == parameter as string).ToList()[0];
        WeakReferenceMessenger.Default.Send(new SearchDetailMessage(current));
        Log.Information($"[Search] Turn to detail page: {current.Name}");
    }

    #endregion Details

    [RelayCommand]
    private async Task ToPreviousPage()
    {
        if (CurrentPage == 1)
        {
            return;
        }
        maskService.Show();
        var result = await packageSearchService.Query(QueryPackageName, CurrentPage - 1);
        Process(result);
        maskService.Hide();
        CurrentPage--;
        DeterminePageReaches();
    }

    [RelayCommand]
    private async Task ToNextPage()
    {
        if (CurrentPage == MaxPage)
        {
            return;
        }
        maskService.Show();
        var result = await packageSearchService.Query(QueryPackageName, CurrentPage + 1);
        Process(result);
        maskService.Hide();
        CurrentPage++;
        DeterminePageReaches();
    }

    private void DeterminePageReaches()
    {
        ReachesFirstPage = CurrentPage == 1;
        ReachesLastPage = CurrentPage == MaxPage;
    }

    private void Process(QueryWrapper queryWrapper)
    {
        if (queryWrapper.Status == QueryStatus.Success)
        {
            foreach (var resultItem in queryWrapper.Results!.Where(resultItem => string.IsNullOrEmpty(resultItem.Description)))
            {
                resultItem.Description = Lang.Search_List_NoDescription;
            }
            QueryList = new ObservableCollection<QueryListItemModel>(queryWrapper.Results!);
            TotalResultNumber = queryWrapper.ResultCount!;
            SuccessQueried = true;
            MaxPage = queryWrapper.MaxPageNumber;
            DeterminePageReaches();
        }
        else
        {
            switch (queryWrapper.Status)
            {
                case QueryStatus.NoResults:
                    toastService.Error(Lang.Search_Query_NoResults);
                    break;
                case QueryStatus.Timeout:
                    toastService.Error(Lang.Search_Query_Timeout);
                    break;
                case QueryStatus.Success:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            QueryList.Clear();
            TotalResultNumber = "";
            SuccessQueried = false;
            MaxPage = 1;
        }
    }

    [RelayCommand]
    private async Task Search(string? parameter)
    {
        if (parameter != null && !string.IsNullOrEmpty(parameter))
        {
            OnQuerying = true;
            Log.Information($"[Search] Query: {parameter}");
            QueryList.Clear();
            TotalResultNumber = "";
            SuccessQueried = false;
            MaxPage = 1;
            CurrentPage = 1;
            QueryPackageName = parameter;
            var result = await packageSearchService.Query(parameter);
            Process(result);
            OnQuerying = false;
        }
    }
}