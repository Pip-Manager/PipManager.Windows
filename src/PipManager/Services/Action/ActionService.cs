using PipManager.Models.Action;
using PipManager.Models.Pages;
using PipManager.Services.Environment;
using Serilog;
using Wpf.Ui;

namespace PipManager.Services.Action;

public class ActionService : IActionService
{
    private readonly IEnvironmentService _environmentService;

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
                    var queue = ActionList[0].OperationCommand.Split(' ');
                    foreach (var item in queue)
                    {
                        ActionList[0].OperationStatus = $"Uninstalling {item}";
                        var result = _environmentService.Uninstall(item);
                        ActionList[0].CompletedSubTaskNumber++;
                        Log.Information(result.Item1
                            ? $"[Runner] {item} uninstall sub-task completed"
                            : $"[Runner] {item} uninstall sub-task failed\n   Reason:{result.Item2}");
                    }
                    Log.Information($"[Runner] Task {ActionList[0].OperationDescription} Completed");
                }
                else if (ActionList[0].OperationType == ActionType.Update)
                {
                    var queue = ActionList[0].OperationCommand.Split(' ');
                    foreach (var item in queue)
                    {
                        ActionList[0].OperationStatus = $"Updating {item}";
                        var result = _environmentService.Update(item);
                        ActionList[0].CompletedSubTaskNumber++;
                        Log.Information(result.Item1
                            ? $"[Runner] {item} update sub-task completed"
                            : $"[Runner] {item} update sub-task failed\n   Reason:{result.Item2}");
                    }
                    Log.Information($"[Runner] Task {ActionList[0].OperationDescription} Completed");
                }

                ActionList[0].CompletedSubTaskNumber = ActionList[0].TotalSubTaskNumber;
                ActionList[0].OperationStatus = "Completed";
                Thread.Sleep(1000);
                ActionList.Remove(ActionList[0]);
            }

            Thread.Sleep(500);
        }
    }
}