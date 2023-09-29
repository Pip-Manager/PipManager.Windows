using PipManager.Services.Configuration;
using PipManager.Views.Pages.Search;
using Wpf.Ui.Appearance;

namespace PipManager.ViewModels.Pages.Library;

public partial class LibraryViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly IConfigurationService _configurationService;

    public LibraryViewModel(INavigationService navigationService, IConfigurationService configurationService)
    {
        _navigationService = navigationService;
        _configurationService = configurationService;
        
        
        Theme.Apply(_configurationService.AppConfig.Personalization.Theme switch
        {
            "light" => ThemeType.Light,
            "dark" => ThemeType.Dark,
            _ => ThemeType.Dark
        });
    }
    [ObservableProperty]
    private int _counter;

    [RelayCommand]
    private void OnCounterIncrement()
    {
        _navigationService.Navigate(typeof(SearchPage));
    }
}