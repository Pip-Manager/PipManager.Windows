using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages;

public partial class DashboardViewModel : ObservableObject
{
    private readonly ISnackbarService _snackbarService;
    public DashboardViewModel(ISnackbarService snackbarService)
    {
        _snackbarService = snackbarService;
    }
    [ObservableProperty]
    private int _counter;

    [RelayCommand]
    private void OnCounterIncrement()
    {
        Counter++;
    }
}