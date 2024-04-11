using PipManager.Languages;
using PipManager.Models.Action;
using PipManager.Services.Environment;
using PipManager.Services.Toast;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Meziantou.Framework.WPF.Collections;

namespace PipManager.Services.Action;

public class ActionService(IEnvironmentService environmentService, IToastService toastService)
    : IActionService
{
    public ConcurrentObservableCollection<ActionListItem> ActionList { get; set; } = [];
    public ObservableCollection<ActionListItem> ExceptionList { get; set; } = [];

    public void AddOperation(ActionListItem actionListItem)
    {
        toastService.Info(string.Format(Lang.Action_AddOperation_Toast, actionListItem.TotalSubTaskNumber));
        ActionList.Add(actionListItem);
    }

    public bool TryCancelOperation(string operationId)
    {
        var targetAction = ActionList.ToList().FindIndex(action => action.OperationId == operationId);
        if (ActionList[targetAction].OperationStatus != Lang.Action_CurrentStatus_WaitingInQueue)
        {
            return environmentService.TryKillProcess();
        }
        ActionList.Remove(ActionList[targetAction]);
        return true;
    }
    
    private static void ConsoleOutputUpdater(ref bool currentActionRunning, ref ActionListItem currentAction, string? data)
    {
        if (!currentActionRunning && !string.IsNullOrEmpty(data))
        {
            currentAction.ConsoleOutput = data.Trim();
            currentActionRunning = true;
        }
        else if (!string.IsNullOrEmpty(data))
        {
            currentAction.ConsoleOutput += '\n' + data.Trim();
        }
    }

    public void Runner()
    {
        while (true)
        {
            if (ActionList.Count > 0)
            {
                var errorDetection = false;
                var consoleError = new StringBuilder(512);
                var currentAction = ActionList[0];
                var currentActionRunning = false;
                currentAction.ConsoleOutput = Lang.Action_CurrentStatus_WaitingInQueue;
                switch (currentAction.OperationType)
                {
                    case ActionType.Uninstall:
                        {
                            var queue = currentAction.OperationCommand;
                            foreach (var item in queue)
                            {
                                currentAction.OperationStatus = $"Uninstalling {item}";
                                var result = environmentService.Uninstall(item, (_, eventArgs) => ConsoleOutputUpdater(ref currentActionRunning, ref currentAction, eventArgs.Data));
                                currentAction.CompletedSubTaskNumber++;
                                Log.Information(result.Success
                                    ? $"[Runner] {item} uninstall sub-task completed"
                                    : $"[Runner] {item} uninstall sub-task failed\n   Reason:{result.Message}");
                            }
                            Log.Information($"[Runner] Task {currentAction.OperationType} Completed");
                            break;
                        }

                    case ActionType.Install:
                        {
                            var queue = currentAction.OperationCommand;
                            foreach (var item in queue)
                            {
                                currentAction.OperationStatus = $"Installing {item}";
                                var result = environmentService.Install(item, (_, eventArgs) => ConsoleOutputUpdater(ref currentActionRunning, ref currentAction, eventArgs.Data), extraParameters: currentAction.ExtraParameters);
                                currentAction.CompletedSubTaskNumber++;
                                if (!result.Success)
                                {
                                    errorDetection = true;
                                    currentAction.DetectIssue = true;
                                    consoleError.AppendLine(result.Message);
                                }
                                Log.Information(result.Success
                                    ? $"[Runner] {item} install sub-task completed"
                                    : $"[Runner] {item} install sub-task failed\n   Reason:{result.Message}");
                            }
                            Log.Information($"[Runner] Task {currentAction.OperationType} Completed");
                            break;
                        }
                    case ActionType.InstallByRequirements:
                        {
                            var requirementsTempFilePath = Path.Combine(AppInfo.CachesDir, $"temp_install_requirements_{currentAction.OperationId}.txt");
                            File.WriteAllText(requirementsTempFilePath, currentAction.OperationCommand[0]);
                            currentAction.OperationStatus = "Installing from requirements.txt";
                            var result = environmentService.InstallByRequirements(requirementsTempFilePath, (_, eventArgs) => ConsoleOutputUpdater(ref currentActionRunning, ref currentAction, eventArgs.Data));
                            if (!result.Success)
                            {
                                errorDetection = true;
                                currentAction.DetectIssue = true;
                                consoleError.AppendLine(result.Message);
                            }
                            Log.Information($"[Runner] Task {currentAction.OperationType} Completed");
                            break;
                        }
                    case ActionType.Download:
                        {
                            var queue = currentAction.OperationCommand;
                            foreach (var item in queue)
                            {
                                currentAction.OperationStatus = $"Downloading {item}";
                                var result = environmentService.Download(item, currentAction.Path, (_, eventArgs) => ConsoleOutputUpdater(ref currentActionRunning, ref currentAction, eventArgs.Data), extraParameters: currentAction.ExtraParameters);
                                currentAction.CompletedSubTaskNumber++;
                                if (!result.Success)
                                {
                                    errorDetection = true;
                                    currentAction.DetectIssue = true;
                                    consoleError.AppendLine(result.Message);
                                }
                                Log.Information(result.Success
                                    ? $"[Runner] {item} download sub-task completed"
                                    : $"[Runner] {item} download sub-task failed\n   Reason:{result.Message}");
                            }
                            Log.Information($"[Runner] Task {currentAction.OperationType} Completed");
                            break;
                        }
                    case ActionType.Update:
                        {
                            var queue = currentAction.OperationCommand;
                            foreach (var item in queue)
                            {
                                currentAction.OperationStatus = $"Updating {item}";
                                var result = environmentService.Update(item, (_, eventArgs) => ConsoleOutputUpdater(ref currentActionRunning, ref currentAction, eventArgs.Data));
                                currentAction.CompletedSubTaskNumber++;
                                if (!result.Success)
                                {
                                    errorDetection = true;
                                    currentAction.DetectIssue = true;
                                    consoleError.AppendLine(result.Message);
                                }
                                Log.Information(result.Success
                                    ? $"[Runner] {item} update sub-task completed"
                                    : $"[Runner] {item} update sub-task failed\n   Reason:{result.Message}");
                            }
                            Log.Information($"[Runner] Task {currentAction.OperationType} Completed");
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                currentAction.CompletedSubTaskNumber = currentAction.TotalSubTaskNumber;
                currentAction.OperationStatus = "Completed";
                if (errorDetection)
                {
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        toastService.Error(Lang.Action_IssueDetectedToast);
                    });
                    currentAction.ConsoleError = consoleError.ToString().TrimEnd();
                    ExceptionList.Add(currentAction);
                }
                Thread.Sleep(100);
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ActionList.RemoveAt(0);
                });
            }
            Thread.Sleep(500);
        }
    }
}