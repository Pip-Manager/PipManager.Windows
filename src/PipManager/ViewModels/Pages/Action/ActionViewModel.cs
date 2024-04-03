using Meziantou.Framework.WPF.Collections;
using PipManager.Languages;
using PipManager.Models.Action;
using PipManager.Services.Action;
using PipManager.Services.Toast;
using PipManager.Views.Pages.Action;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Action;

public partial class ActionViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty]
    private ConcurrentObservableCollection<ActionListItem> _actions;

    private readonly IActionService _actionService;
    private readonly IToastService _toastService;
    private readonly INavigationService _navigationService;

    public ActionViewModel(IActionService actionService, INavigationService navigationService, IToastService toastService)
    {
        _actionService = actionService;
        _navigationService = navigationService;
        _toastService = toastService;
        Actions = _actionService.ActionList;
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
        Log.Information("[Action] Initialized");
    }

    [RelayCommand]
    private void ShowExceptions()
    {
        _navigationService.NavigateWithHierarchy(typeof(ActionExceptionPage));
    }

    [RelayCommand]
    private void CancelAction(string? operationId)
    {
        if (string.IsNullOrEmpty(operationId))
        {
            return;
        }

        var result = _actionService.TryCancelOperation(operationId);
        if(result == Lang.Action_OperationCanceled_AlreadyRunning)
        {
            _toastService.Error(Lang.Action_OperationCanceled_AlreadyRunning);
            Log.Warning("[Action] Operation cancellation failed (already running): {OperationId}", operationId);
        }
        else
        {
            _toastService.Success(Lang.Action_OperationCanceled_Success);
            Log.Information("[Action] Operation canceled: {OperationId}", operationId);
        }
    }
}