using PipManager.Models.Action;
using PipManager.Services.Action;
using PipManager.Services.Overlay;
using Serilog;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Lab;

public partial class LabViewModel(IActionService actionService, IOverlayService overlayService)
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

    [RelayCommand]
    private void OverlayTest()
    {
        overlayService.ShowOverlay();
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