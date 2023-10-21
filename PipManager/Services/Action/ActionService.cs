using System.Collections.ObjectModel;
using System.Windows.Threading;
using PipManager.Models.Pages;
using PipManager.Services.Environment;
using Serilog;

namespace PipManager.Services.Action;

public class ActionService: IActionService
{
    private IEnvironmentService _environmentService;
    public ActionService(IEnvironmentService environmentService)
    {
        _environmentService = environmentService;
    }

    public List<ActionListItem> ActionList { get; set; } = new();

    public void Runner()
    {
        while (true)
        {
            if (ActionList.Count > 0)
            {
                if (ActionList[0].OperationType == ActionType.Uninstall)
                {
                    ActionList[0].OperationStatus = "Action Started";
                    var queue = ActionList[0].OperationCommand.Split(' ');
                    foreach (var item in queue)
                    {
                        ActionList[0].OperationStatus = $"Uninstalling {item}";
                        var result = _environmentService.Uninstall(item);
                        if (result)
                        {
                            ActionList[0].CompletedSubTaskNumber++;
                            Log.Information($"[Runner] Uninstalled {item}");
                        }
                    }
                    Log.Information($"[Runner] Task {ActionList[0].OperationDescription} Completed");
                }
                ActionList.Remove(ActionList[0]);
                
            }

            Thread.Sleep(3000);
        }
    }
}