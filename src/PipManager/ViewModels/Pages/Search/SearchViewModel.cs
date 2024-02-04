using PipManager.Languages;
using PipManager.PackageSearch;
using PipManager.PackageSearch.Wrappers.Query;
using PipManager.Services.Mask;
using PipManager.Services.Toast;
using Serilog;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Search;

public partial class SearchViewModel(IPackageSearchService packageSearchService, IToastService toastService, IMaskService maskService) : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty]
    private ObservableCollection<QueryListItemModel> _queryList = [];
    [ObservableProperty]
    private string _queryPackageName = "";
    [ObservableProperty]
    private string _totalResultNumber = "";
    [ObservableProperty]
    private bool _successQueried = false;
    [ObservableProperty]
    private bool _reachesFirstPage = true;
    [ObservableProperty]
    private bool _reachesLastPage = false;
    [ObservableProperty]
    private int _currentPage = 1;
    [ObservableProperty]
    private int _maxPage = 0;

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

    [RelayCommand]
    public async Task ToPreviousPage()
    {
        if(CurrentPage == 1)
        {
            return;
        }
        maskService.Show();
        var result = await packageSearchService.Query(QueryPackageName, CurrentPage-1);
        Process(result);
        maskService.Hide();
        CurrentPage--;
        DeterminePageReaches();
    }

    [RelayCommand]
    public async Task ToNextPage()
    {
        if(CurrentPage == MaxPage)
        {
            return;
        }
        maskService.Show();
        var result = await packageSearchService.Query(QueryPackageName, CurrentPage+1);
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
            foreach (var resultItem in queryWrapper.Results!)
            {
                if (string.IsNullOrEmpty(resultItem.Description))
                {
                    resultItem.Description = Lang.Search_List_NoDescription;
                }
            }
            QueryList = new ObservableCollection<QueryListItemModel>(queryWrapper.Results!);
            TotalResultNumber = queryWrapper.ResultCount!;
            SuccessQueried = true;
            MaxPage = queryWrapper.MaxPageNumber;
            DeterminePageReaches();
        }
        else
        {
            if (queryWrapper.Status == QueryStatus.NoResults)
            {
                toastService.Error(Lang.Search_Query_NoResults);
            }
            else if (queryWrapper.Status == QueryStatus.Timeout)
            {
                toastService.Error(Lang.Search_Query_Timeout);
            }
            QueryList.Clear();
            TotalResultNumber = "";
            SuccessQueried = false;
            MaxPage = 0;
        }
    }

    [RelayCommand]
    public async Task Search(string? parameter)
    {
        if (parameter != null && !string.IsNullOrEmpty(parameter))
        {
            QueryList.Clear();
            TotalResultNumber = "";
            SuccessQueried = false;
            MaxPage = 0;
            CurrentPage = 1;
            QueryPackageName = parameter;
            var result = await packageSearchService.Query(parameter, 1);
            Process(result);
        }
    }
}