using PipManager.Languages;
using Wpf.Ui.Controls;

namespace PipManager.Models.Action;

public partial class ActionListItem : ObservableObject
{
    public ActionListItem(ActionType operationType, string[] operationCommand, string displayCommand = "", string path = "", string[]? extraParameters = null, bool progressIntermediate = false)
    {
        OperationType = operationType;
        OperationCommand = operationCommand;
        ProgressIntermediate = progressIntermediate;
        TotalSubTaskNumber = operationCommand.Length;
        Path = path;
        ExtraParameters = extraParameters;
        DisplayCommand = displayCommand switch
        {
            "" => string.Join(' ', operationCommand),
            _ => displayCommand,
        };
        OperationDescription = operationType switch
        {
            ActionType.Uninstall => Lang.Action_Operation_Uninstall,
            ActionType.Install => Lang.Action_Operation_Install,
            ActionType.InstallByRequirements => Lang.Action_Operation_InstallByRequirements,
            ActionType.Download => Lang.Action_Operation_Download,
            ActionType.Update => Lang.Action_Operation_Update,
            _ => "Unknown",
        };

        OperationIcon = operationType switch
        {
            ActionType.Uninstall => new SymbolIcon(SymbolRegular.Delete24),
            ActionType.Install or ActionType.InstallByRequirements => new SymbolIcon(SymbolRegular.Add24),
            ActionType.Download => new SymbolIcon(SymbolRegular.ArrowDownload24),
            ActionType.Update => new SymbolIcon(SymbolRegular.ArrowUp24),
            _ => new SymbolIcon(SymbolRegular.Question24),
        };
        BadgeAppearance = operationType switch
        {
            ActionType.Uninstall => "Danger",
            ActionType.Install or ActionType.InstallByRequirements => "Success",
            ActionType.Update or ActionType.Download => "Caution",
            _ => "Primary",
        };
        ConsoleOutput = "";
        ConsoleError = Lang.Action_ConsoleError_Empty;
        Completed = false;
    }

    #region Initialization Required

    public string OperationId { get; set; } = Guid.NewGuid().ToString();
    public SymbolIcon OperationIcon { get; set; }
    public ActionType OperationType { get; set; }
    public string OperationDescription { get; set; }
    public string OperationTimestamp { get; set; } = DateTime.Now.ToLocalTime().ToString("yyyy-M-d HH:mm:ss");
    public string[] OperationCommand { get; set; }
    public string DisplayCommand { get; set; }
    public string Path { get; set; }
    public string[]? ExtraParameters { get; set; }
    public bool ProgressIntermediate { get; set; }
    public string BadgeAppearance { get; set; }

    #endregion Initialization Required

    #region Property Update Required

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressBarValue))]
    private int _totalSubTaskNumber;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressBarValue))]
    private int _completedSubTaskNumber;

    public double ProgressBarValue
    {
        get => (double)CompletedSubTaskNumber / TotalSubTaskNumber * 100.0;
    }

    [ObservableProperty] private string _operationStatus = Lang.Action_CurrentStatus_WaitingInQueue;
    [ObservableProperty] private string _consoleOutput = Lang.Action_CurrentStatus_WaitingInQueue;
    [ObservableProperty] private string _consoleError;
    [ObservableProperty] private bool _completed;
    [ObservableProperty] private bool _detectIssue;

    #endregion Property Update Required
}