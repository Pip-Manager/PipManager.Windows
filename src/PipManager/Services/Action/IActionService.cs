using PipManager.Models.Action;
using PipManager.Models.Pages;

namespace PipManager.Services.Action;

public interface IActionService
{
    public List<ActionListItem> ActionList { get; set; }
    public List<ActionListItem> ExceptionList { get; set; }
    public void AddOperation(ActionListItem actionListItem);
    public void Runner();
}