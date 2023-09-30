using PipManager.Services.Configuration;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.About;

public partial class AboutViewModel : ObservableObject, INavigationAware
{
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
        AppVersion = AppInfo.AppVersion;
        _isInitialized = true;
    }

}