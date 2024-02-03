using PipManager.Languages;
using PipManager.Models.Action;
using PipManager.Services.Environment;
using PipManager.Services.Toast;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;

namespace PipManager.Services.Action;

public class ActionService(IEnvironmentService environmentService, IToastService toastService)
    : IActionService
{
    public ObservableCollection<ActionListItem> ActionList { get; set; } = new();
    public ObservableCollection<ActionListItem> ExceptionList { get; set; } = new();

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
                var currentAction = ActionList[0];
                switch (currentAction.OperationType)
                {
                    case ActionType.Uninstall:
                        {
                            var queue = currentAction.OperationCommand.Split(' ');
                            foreach (var item in queue)
                            {
                                currentAction.OperationStatus = $"Uninstalling {item}";
                                var result = environmentService.Uninstall(item, (sender, eventArgs) =>
                                {
                                    if (!string.IsNullOrEmpty(eventArgs.Data))
                                    {
                                        currentAction.ConsoleOutput = eventArgs.Data;
                                    }
                                });
                                currentAction.CompletedSubTaskNumber++;
                                Log.Information(result.Item1
                                    ? $"[Runner] {item} uninstall sub-task completed"
                                    : $"[Runner] {item} uninstall sub-task failed\n   Reason:{result.Item2}");
                            }
                            Log.Information($"[Runner] Task {currentAction.OperationDescription} Completed");
                            break;
                        }
                    case ActionType.InstallByRequirements:
                        {
                            var requirementsTempFilePath = Path.Combine(AppInfo.CachesDir, $"{currentAction.OperationId}_requirements.txt");
                            File.WriteAllText(requirementsTempFilePath, currentAction.OperationCommand);
                            currentAction.OperationStatus = $"Installing from requirements.txt";
                            var result = environmentService.InstallByRequirements(requirementsTempFilePath, (sender, eventArgs) =>
                            {
                                if (!string.IsNullOrEmpty(eventArgs.Data))
                                {
                                    currentAction.ConsoleOutput = eventArgs.Data;
                                }
                            });
                            if (!result.Item1)
                            {
                                errorDetection = true;
                                currentAction.DetectIssue = true;
                                consoleError += result.Item2 + '\n';
                            }
                            Log.Information($"[Runner] Task {currentAction.OperationDescription} Completed");
                            break;
                        }
                    case ActionType.Install:
                        {
                            var queue = currentAction.OperationCommand.Split(' ');
                            foreach (var item in queue)
                            {
                                currentAction.OperationStatus = $"Installing {item}";
                                var result = environmentService.Install(item, (sender, eventArgs) =>
                                {
                                    if (!string.IsNullOrEmpty(eventArgs.Data))
                                    {
                                        currentAction.ConsoleOutput = eventArgs.Data;
                                    }
                                });
                                currentAction.CompletedSubTaskNumber++;
                                if (!result.Item1)
                                {
                                    errorDetection = true;
                                    currentAction.DetectIssue = true;
                                    consoleError += result.Item2 + '\n';
                                }
                                Log.Information(result.Item1
                                    ? $"[Runner] {item} install sub-task completed"
                                    : $"[Runner] {item} install sub-task failed\n   Reason:{result.Item2}");
                            }
                            Log.Information($"[Runner] Task {currentAction.OperationDescription} Completed");
                            break;
                        }
                    case ActionType.Update:
                        {
                            var queue = currentAction.OperationCommand.Split(' ');
                            foreach (var item in queue)
                            {
                                currentAction.OperationStatus = $"Updating {item}";
                                var result = environmentService.Update(item, (sender, eventArgs) =>
                                {
                                    if (!string.IsNullOrEmpty(eventArgs.Data))
                                    {
                                        currentAction.ConsoleOutput = eventArgs.Data;
                                    }
                                });
                                currentAction.CompletedSubTaskNumber++;
                                if (!result.Item1)
                                {
                                    errorDetection = true;
                                    currentAction.DetectIssue = true;
                                    consoleError += result.Item2 + '\n';
                                }
                                Log.Information(result.Item1
                                    ? $"[Runner] {item} update sub-task completed"
                                    : $"[Runner] {item} update sub-task failed\n   Reason:{result.Item2}");
                            }
                            Log.Information($"[Runner] Task {currentAction.OperationDescription} Completed");
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
                    currentAction.ConsoleError = consoleError;
                    ExceptionList.Add(currentAction);
                }
                Thread.Sleep(500);

                Application.Current.Dispatcher.Invoke(delegate
                {
                    ActionList.RemoveAt(0);
                });

            }
        }
    }
}