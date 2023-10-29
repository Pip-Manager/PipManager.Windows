using PipManager.Languages;
using Wpf.Ui.Controls;

namespace PipManager.Models.Pages;

public class ActionListItem
{
    public ActionListItem(ActionType operationType, string operationDescription, string operationCommand, string operationStatus = "Waiting in queue", bool progressIntermediate = false, int totalSubTaskNumber = 0, int completedSubTaskNumber = 0)
    {
        OperationType = operationType;
        OperationDescription = operationDescription;
        OperationCommand = operationCommand;
        OperationStatus = operationStatus;
        ProgressIntermediate = progressIntermediate;
        TotalSubTaskNumber = totalSubTaskNumber;
        CompletedSubTaskNumber = completedSubTaskNumber;
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

    public SymbolIcon OperationIcon { get; set; }
    public ActionType OperationType { get; set; }
    public string OperationDescription { get; set; }
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
}

