using PipManager.Models;
using Serilog;
using System.Windows.Media;
using PipManager.Models.EnvironmentModels;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Environment;

public partial class EnvironmentViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty] private List<PipItemModel> _pipItems;

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
        PipItems = new List<PipItemModel>
        {
            new("Main environment", "23.1.2", "3.11", "C:\\Users\\Mccree Lee\\AppData\\Local\\Programs\\Python\\Python311\\Lib\\site-packages\\pip")
        };
    }
}