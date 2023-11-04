using PipManager.Languages;
using Wpf.Ui.Controls;

namespace PipManager.Models.Action;

public class ActionListItem
{
    public ActionListItem(ActionType operationType, string operationCommand, string operationStatus = "Waiting in queue", bool progressIntermediate = false, int totalSubTaskNumber = 0, int completedSubTaskNumber = 0)
    {
        OperationType = operationType;
        OperationCommand = operationCommand;
        OperationStatus = operationStatus;
        ProgressIntermediate = progressIntermediate;
        TotalSubTaskNumber = totalSubTaskNumber;
        CompletedSubTaskNumber = completedSubTaskNumber;
        OperationDescription = operationType switch
        {
            ActionType.Uninstall => Lang.Action_Operation_Uninstall,
            ActionType.Install => Lang.Action_Operation_Install,
            ActionType.Update => Lang.Action_Operation_Update,
            _ => "Unknown",
        };

        OperationIcon = operationType switch
        {
            ActionType.Uninstall => new SymbolIcon(SymbolRegular.Delete24),
            ActionType.Install => new SymbolIcon(SymbolRegular.Add24),
            ActionType.Update => new SymbolIcon(SymbolRegular.ArrowUp24),
            _ => new SymbolIcon(SymbolRegular.Question24),
        };
        BadgeAppearance = operationType switch
        {
            ActionType.Uninstall => "Danger",
            ActionType.Install => "Success",
            ActionType.Update => "Caution",
            _ => "Primary",
        };
        ProgressBarValue = 0;
        ConsoleError = Lang.Action_ConsoleError_Empty;
        Completed = false;
    }

    public string OperationId { get; } = Guid.NewGuid().ToString();

    public SymbolIcon OperationIcon { get; set; }
    public ActionType OperationType { get; set; }
    public string OperationDescription { get; set; }
    public string OperationTimestamp { get; } = DateTime.Now.ToLocalTime().ToString("yyyy-M-d HH:mm:ss");
    public string OperationCommand { get; set; }
    public string OperationStatus { get; set; }
    public bool ProgressIntermediate { get; set; }
    public int TotalSubTaskNumber { get; set; }

    private int completedSubTaskNumber;

    public int CompletedSubTaskNumber
    {
        get => completedSubTaskNumber;
        set
        {
            completedSubTaskNumber = value;
            ProgressBarValue = (double)value / TotalSubTaskNumber * 100.0;
        }
    }

    public string ConsoleError { get; set; }

    public string BadgeAppearance { get; set; }
    public double ProgressBarValue { get; set; }
    public bool Completed { get; set; }
    public bool DetectIssue { get; set; }
}