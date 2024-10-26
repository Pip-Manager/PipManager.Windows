using Serilog;
using Wpf.Ui.Abstractions.Controls;

namespace PipManager.Windows.ViewModels.Pages.Tools;

public partial class ToolsViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    
    [ObservableProperty]
    private string? _testProperty;

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Tools] Initialized");
    }

    public Task OnNavigatedToAsync()
    {
        if (!_isInitialized)
            InitializeViewModel();
        return Task.CompletedTask;
    }

    public Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }
}