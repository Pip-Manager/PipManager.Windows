using CommunityToolkit.Mvvm.Messaging;
using PipManager.PackageSearch.Wrappers.Query;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Search;

public partial class SearchDetailViewModel : ObservableObject, INavigationAware
{
    public record SearchDetailMessage(QueryListItemModel Package);
    private bool _isInitialized;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private QueryListItemModel? _package;

    public SearchDetailViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        WeakReferenceMessenger.Default.Register<SearchDetailMessage>(this, Receive);
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        _navigationService.GetNavigationControl().BreadcrumbBar!.Visibility = Visibility.Collapsed;
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
    }
}