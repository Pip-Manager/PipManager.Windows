using System.Collections.ObjectModel;
using PipManager.Models;
using Serilog;
using System.Windows.Media;
using PipManager.Models.Pages;
using PipManager.Services.Action;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Action;

public partial class ActionViewModel : ObservableObject, INavigationAware
{
    private bool _isInitialized;
    [ObservableProperty]
    private ObservableCollection<ActionListItem> _actions;

    private IActionService _actionService;

    public ActionViewModel(IActionService actionService)
    {
        _actionService = actionService;
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
            InitializeViewModel();
        Actions = new ObservableCollection<ActionListItem>(_actionService.ActionList);
    }

    public void OnNavigatedFrom()
    {
    }

    private void InitializeViewModel()
    {
        _isInitialized = true;
        Log.Information("[Action] Initialized");
    }
}