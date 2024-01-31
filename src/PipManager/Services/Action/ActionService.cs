using PipManager.Languages;
using PipManager.Models.Action;
using PipManager.Services.Environment;
using PipManager.Services.Toast;
using Serilog;

namespace PipManager.Services.Action;

public class ActionService(IEnvironmentService environmentService, IToastService toastService)
    : IActionService
{
    public List<ActionListItem> ActionList { get; set; } = new();
    public List<ActionListItem> ExceptionList { get; set; } = new();

    public void AddOperation(ActionListItem actionListItem)
    {
        toastService.Info(string.Format(Lang.Action_AddOperation_Toast, actionListItem.TotalSubTaskNumber));
        ActionList.Add(actionListItem);
    }

    public void Runner()
    {
        while (true)
        {
            if (ActionList.Count > 0)
            {
                var errorDetection = false;
                var consoleError = "\n";
                switch (ActionList[0].OperationType)
                {
                    case ActionType.Uninstall:
                    {
                        var queue = ActionList[0].OperationCommand.Split(' ');
                        foreach (var item in queue)
                        {
                            ActionList[0].OperationStatus = $"Uninstalling {item}";
                            var result = environmentService.Uninstall(item);
                            ActionList[0].CompletedSubTaskNumber++;
                            Log.Information(result.Item1
                                ? $"[Runner] {item} uninstall sub-task completed"
                                : $"[Runner] {item} uninstall sub-task failed\n   Reason:{result.Item2}");
                        }
                        Log.Information($"[Runner] Task {ActionList[0].OperationDescription} Completed");
                        break;
                    }
                    case ActionType.Install:
                    {
                        var queue = ActionList[0].OperationCommand.Split(' ');
                        foreach (var item in queue)
                        {
                            ActionList[0].OperationStatus = $"Installing {item}";
                            var result = environmentService.Install(item);
                            ActionList[0].CompletedSubTaskNumber++;
                            if (!result.Item1)
                            {
                                errorDetection = true;
                                ActionList[0].DetectIssue = true;
                                consoleError += result.Item2 + '\n';
                            }
                            Log.Information(result.Item1
                                ? $"[Runner] {item} install sub-task completed"
                                : $"[Runner] {item} install sub-task failed\n   Reason:{result.Item2}");
                        }
                        Log.Information($"[Runner] Task {ActionList[0].OperationDescription} Completed");
                        break;
                    }
                    case ActionType.Update:
                    {
                        var queue = ActionList[0].OperationCommand.Split(' ');
                        foreach (var item in queue)
                        {
                            ActionList[0].OperationStatus = $"Updating {item}";
                            var result = environmentService.Update(item);
                            ActionList[0].CompletedSubTaskNumber++;
                            if (!result.Item1)
                            {
                                errorDetection = true;
                                ActionList[0].DetectIssue = true;
                                consoleError += result.Item2 + '\n';
                            }
                            Log.Information(result.Item1
                                ? $"[Runner] {item} update sub-task completed"
                                : $"[Runner] {item} update sub-task failed\n   Reason:{result.Item2}");
                        }
                        Log.Information($"[Runner] Task {ActionList[0].OperationDescription} Completed");
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                ActionList[0].CompletedSubTaskNumber = ActionList[0].TotalSubTaskNumber;
                ActionList[0].OperationStatus = "Completed";
                if (errorDetection)
                {
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        toastService.Error(Lang.Action_IssueDetectedToast);
                    });
                    ActionList[0].ConsoleError = consoleError;
                    ExceptionList.Add(ActionList[0]);
                }
                Thread.Sleep(1000);
                ActionList.RemoveAt(0);
            }
            Thread.Sleep(500);
        }
    }
}