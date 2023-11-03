using PipManager.Models.Pages;
using PipManager.Services.Action;
using Serilog;
using System.Collections.ObjectModel;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Action;

public partial class ActionViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [ObservableProperty]
    private ObservableCollection<ActionListItem> _actions;

    private readonly IActionService _actionService;

    public ActionViewModel(IActionService actionService)
    {
        _actionService = actionService;
        Actions = new ObservableCollection<ActionListItem>(_actionService.ActionList);
        _ = UpdateActionListTask();
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

    public async Task UpdateActionListTask()
    {
        while (true)
        {
            Actions = new ObservableCollection<ActionListItem>(_actionService.ActionList);
            await Task.Delay(100);
        }
    }
}