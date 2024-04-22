using PipManager.Models.Action;
using PipManager.Services.Action;
using PipManager.Services.Environment;
using Serilog;
using Wpf.Ui.Controls;

namespace PipManager.ViewModels.Pages.Lab;

public partial class LabViewModel(IActionService actionService, IEnvironmentService environmentService)
    : ObservableObject, INavigationAware
{
    private bool _isInitialized;

    [RelayCommand]
    private void ParseTest()
    {
        var parsed = environmentService.ParseRequirements(["requests", "numpy"]);
        parsed.Requirements?.ForEach(item => Log.Information(item.Specifier));
    }

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