using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace PipManager.Models.Pages;

public class ActionListItem
{
    public ActionListItem(ActionType operationType, string operationDescription, string operationCommand, bool progressIntermediate = true, int totalSubTaskNumber = 0, int completedSubTaskNumber = 0)
    {
        OperationType = operationType;
        OperationDescription = operationDescription;
        OperationCommand = operationCommand;
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
    }

    public SymbolIcon OperationIcon { get; set; }
    public ActionType OperationType { get; set; }
    public string OperationDescription { get; set; }
    public string OperationCommand { get; set; }
    public bool ProgressIntermediate { get; set; }
    public int TotalSubTaskNumber { get; set; }
    public int CompletedSubTaskNumber { get; set; }

    public string BadgeAppearance { get; set; }
}