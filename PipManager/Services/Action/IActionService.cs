using PipManager.Models.Pages;

namespace PipManager.Services.Action;

public interface IActionService
{
    public List<ActionListItem> ActionList { get; set; }

}