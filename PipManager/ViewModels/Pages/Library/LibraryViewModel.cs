using PipManager.Services.Configuration;
using Serilog;
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

        Log.Information("[Library] Initialized");
    }

    [ObservableProperty]
    private int _counter;

    [RelayCommand]
    private void OnCounterIncrement()
    {
    }
}