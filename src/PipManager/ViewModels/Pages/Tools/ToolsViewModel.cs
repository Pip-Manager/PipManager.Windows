using Serilog;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Tools;

public partial class ToolsViewModel : ObservableObject, INavigationAware
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
        Log.Information("[Tools] Initialized");
    }
}