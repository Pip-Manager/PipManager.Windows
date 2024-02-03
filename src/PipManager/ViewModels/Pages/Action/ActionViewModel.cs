using PipManager.Models.Action;
using PipManager.Services.Action;
using PipManager.Views.Pages.Action;
using Serilog;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Action;

public partial class ActionViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty]
    private ObservableCollection<ActionListItem> _actions;

    private readonly IActionService _actionService;
    private readonly INavigationService _navigationService;

    public ActionViewModel(IActionService actionService, INavigationService navigationService)
    {
        _actionService = actionService;
        _navigationService = navigationService;
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
}