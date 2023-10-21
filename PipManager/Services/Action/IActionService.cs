using PipManager.Models.Pages;
using System.Collections.ObjectModel;

namespace PipManager.Services.Action;

public interface IActionService
{
    public List<ActionListItem> ActionList { get; set; }
    public void Runner();

}