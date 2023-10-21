using PipManager.Models.Pages;

namespace PipManager.Services.Action;

public class ActionService: IActionService
{
    public List<ActionListItem> ActionList { get; set; } = new();
}