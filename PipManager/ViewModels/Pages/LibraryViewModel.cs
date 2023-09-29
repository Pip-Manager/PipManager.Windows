using PipManager.Services.Configuration;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages;

public partial class LibraryViewModel : ObservableObject
{
    private readonly ISnackbarService _snackbarService;
    private readonly IConfigurationService _configurationService;

    public LibraryViewModel(ISnackbarService snackbarService, IConfigurationService configurationService)
    {
        _snackbarService = snackbarService;
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
        Counter++;
    }
}