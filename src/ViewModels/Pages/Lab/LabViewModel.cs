using PipManager.Windows.Models.Action;
using PipManager.Windows.Services.Action;
using Serilog;
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;

namespace PipManager.Windows.ViewModels.Pages.Lab;

public partial class LabViewModel(IActionService actionService)
    : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [RelayCommand]
    private void ActionTest()
    {
        actionService.AddOperation(new ActionListItem
        (
            ActionType.Install,
            ["pytorch"],
            progressIntermediate: false
        ));
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Lab] Initialized");
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