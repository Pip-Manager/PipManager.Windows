using PipManager.Views.Pages.Environment;
using Serilog;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Environment;

public partial class EnvironmentViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService;

    public EnvironmentViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

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
        Log.Information("[Environment] Initialized");
    }

    #region Add Environment

    [RelayCommand]
    private void AddEnvironment()
    {
        _navigationService.Navigate(typeof(AddEnvironmentPage));
    }

    #endregion
}