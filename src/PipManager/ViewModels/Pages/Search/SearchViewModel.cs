using PipManager.Models;
using Serilog;
using System.Windows.Media;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Search;

public partial class SearchViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

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
}