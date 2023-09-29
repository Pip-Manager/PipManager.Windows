using PipManager.Services.Configuration;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.About;

public partial class AboutViewModel : ObservableObject, INavigationAware
{
    private readonly IConfigurationService _configurationService;
    public AboutViewModel(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }
    private bool _isInitialized;

    [ObservableProperty] private string _appVersion = string.Empty;

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
        AppVersion = _configurationService.AppVersion;
        _isInitialized = true;
    }

}