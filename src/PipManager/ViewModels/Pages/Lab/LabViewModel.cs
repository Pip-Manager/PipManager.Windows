using PipManager.Models.Action;
using PipManager.Services.Action;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Lab;

public partial class LabViewModel(INavigationService navigationService, IActionService actionService)
    : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    private readonly INavigationService _navigationService = navigationService;

    [RelayCommand]
    private void ActionTest()
    {
        actionService.AddOperation(new ActionListItem
        (
            ActionType.Install,
            "torch requests a pyqt6",
            totalSubTaskNumber: 4,
            progressIntermediate: false
        ));
    }

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
        Log.Information("[Lab] Initialized");
    }
}